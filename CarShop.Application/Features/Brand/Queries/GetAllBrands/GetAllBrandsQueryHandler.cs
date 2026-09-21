using CarShop.Application.Interfaces;
using CarShop.Application.DTOs.Brand;
using CarShop.Application.Interfaces.Cache;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Brand.Queries.GetAllBrands
{
    public class GetAllBrandsQueryHandler : IRequestHandler<GetAllBrandsQuery, Result<IEnumerable<BrandDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICacheService _cacheService;

        private const string AllBrandsKey = "brands:all";

        public GetAllBrandsQueryHandler(IApplicationDbContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<Result<IEnumerable<BrandDto>>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
        {
            var redis = await _context.IntegrationSettings.Where(s => s.ServiceName == "Redis")
                .Select(s => new { s.IsEnabled }).FirstOrDefaultAsync(cancellationToken);
            var isRedisEnabled = redis != null && redis.IsEnabled;
            if (isRedisEnabled)
            {
                var cached = await _cacheService.GetAsync<IEnumerable<BrandDto>>(AllBrandsKey);
                if (cached != null)
                    return Result<IEnumerable<BrandDto>>.Ok(cached);
            }

            var brands = await _context.Brands.ToListAsync();
            var result = brands.Select(b => new BrandDto { Id = b.Id, Name = b.Name });
            if (isRedisEnabled)
                await _cacheService.SetAsync(AllBrandsKey, result, TimeSpan.FromDays(1));
            return Result<IEnumerable<BrandDto>>.Ok(result);
        }
    }
}
