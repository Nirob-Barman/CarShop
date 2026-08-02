using CarShop.Application.DTOs.Brand;
using CarShop.Application.Interfaces.Cache;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using MediatR;
using BrandEntity = CarShop.Domain.Entities.Brand;

namespace CarShop.Application.Features.Brand.Queries.GetAllBrands
{
    public class GetAllBrandsQueryHandler : IRequestHandler<GetAllBrandsQuery, Result<IEnumerable<BrandDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        private const string AllBrandsKey = "brands:all";

        public GetAllBrandsQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<Result<IEnumerable<BrandDto>>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
        {
            var redis = await _unitOfWork.Repository<IntegrationSetting>()
                .FirstOrDefaultAsync(s => s.ServiceName == "Redis", s => new { s.IsEnabled });
            var isRedisEnabled = redis != null && redis.IsEnabled;
            if (isRedisEnabled)
            {
                var cached = await _cacheService.GetAsync<IEnumerable<BrandDto>>(AllBrandsKey);
                if (cached != null)
                    return Result<IEnumerable<BrandDto>>.Ok(cached);
            }

            var brands = await _unitOfWork.Repository<BrandEntity>().GetAllAsync();
            var result = brands.Select(b => new BrandDto { Id = b.Id, Name = b.Name });
            if (isRedisEnabled)
                await _cacheService.SetAsync(AllBrandsKey, result, TimeSpan.FromDays(1));
            return Result<IEnumerable<BrandDto>>.Ok(result);
        }
    }
}
