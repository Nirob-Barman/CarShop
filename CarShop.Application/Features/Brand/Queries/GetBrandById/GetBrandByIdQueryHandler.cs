using CarShop.Application.Interfaces;
using CarShop.Application.DTOs.Brand;
using CarShop.Application.Interfaces.Cache;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Brand.Queries.GetBrandById
{
    public class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, Result<BrandDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICacheService _cacheService;

        private static string BrandKey(int id) => $"brands:{id}";

        public GetBrandByIdQueryHandler(IApplicationDbContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<Result<BrandDto>> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
        {
            var redis = await _context.IntegrationSettings.AsNoTracking()
                .Where(s => s.ServiceName == "Redis")
                .Select(s => new { s.IsEnabled })
                .FirstOrDefaultAsync(cancellationToken);
            var isRedisEnabled = redis != null && redis.IsEnabled;
            if (isRedisEnabled)
            {
                var cached = await _cacheService.GetAsync<BrandDto>(BrandKey(request.Id));
                if (cached != null)
                    return Result<BrandDto>.Ok(cached);
            }

            var brand = await _context.Brands.FindAsync(request.Id);
            if (brand == null)
                return Result<BrandDto>.Fail("Brand not found");

            var dto = new BrandDto { Id = brand.Id, Name = brand.Name };
            if (isRedisEnabled)
                await _cacheService.SetAsync(BrandKey(request.Id), dto, TimeSpan.FromMinutes(10));
            return Result<BrandDto>.Ok(dto);
        }
    }
}
