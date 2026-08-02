using CarShop.Application.DTOs.Brand;
using CarShop.Application.Interfaces.Cache;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using BrandEntity = CarShop.Domain.Entities.Brand;

namespace CarShop.Application.Features.Brand.Queries.GetBrandById
{
    public class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, Result<BrandDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        private static string BrandKey(int id) => $"brands:{id}";

        public GetBrandByIdQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<Result<BrandDto>> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
        {
            var redis = await _unitOfWork.Repository<CarShop.Domain.Entities.IntegrationSetting>()
                .FirstOrDefaultAsync(s => s.ServiceName == "Redis", s => new { s.IsEnabled });
            var isRedisEnabled = redis != null && redis.IsEnabled;
            if (isRedisEnabled)
            {
                var cached = await _cacheService.GetAsync<BrandDto>(BrandKey(request.Id));
                if (cached != null)
                    return Result<BrandDto>.Ok(cached);
            }

            var brand = await _unitOfWork.Repository<BrandEntity>().GetByIdAsync(request.Id);
            if (brand == null)
                return Result<BrandDto>.Fail("Brand not found");

            var dto = new BrandDto { Id = brand.Id, Name = brand.Name };
            if (isRedisEnabled)
                await _cacheService.SetAsync(BrandKey(request.Id), dto, TimeSpan.FromMinutes(10));
            return Result<BrandDto>.Ok(dto);
        }
    }
}
