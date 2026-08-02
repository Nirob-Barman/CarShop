using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Interfaces.Cache;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using MediatR;
using PromoCodeEntity = CarShop.Domain.Entities.PromoCode;

namespace CarShop.Application.Features.PromoCode.Queries.GetActivePromoCodes
{
    public class GetActivePromoCodesQueryHandler : IRequestHandler<GetActivePromoCodesQuery, Result<IEnumerable<PromoCodeDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        private const string ActiveCodesKey = "promos:active";

        public GetActivePromoCodesQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<Result<IEnumerable<PromoCodeDto>>> Handle(GetActivePromoCodesQuery request, CancellationToken cancellationToken)
        {
            var redis = await _unitOfWork.Repository<IntegrationSetting>()
                .FirstOrDefaultAsync(s => s.ServiceName == "Redis", s => new { s.IsEnabled });
            var isRedisEnabled = redis != null && redis.IsEnabled;

            if (isRedisEnabled)
            {
                var cached = await _cacheService.GetAsync<IEnumerable<PromoCodeDto>>(ActiveCodesKey);
                if (cached != null)
                    return Result<IEnumerable<PromoCodeDto>>.Ok(cached);
            }

            var now   = DateTime.UtcNow;
            var codes = await _unitOfWork.Repository<PromoCodeEntity>().GetAllAsync(
                p => p.IsActive &&
                     (!p.MaxUsages.HasValue || p.UsageCount < p.MaxUsages.Value) &&
                     (!p.ExpiresAt.HasValue || p.ExpiresAt.Value > now),
                p => new PromoCodeDto
                {
                    Id                = p.Id,
                    Code              = p.Code,
                    DiscountPercent   = p.DiscountPercent,
                    MaxDiscountAmount = p.MaxDiscountAmount,
                    MaxUsages         = p.MaxUsages,
                    UsageCount        = p.UsageCount,
                    ExpiresAt         = p.ExpiresAt,
                    IsActive          = p.IsActive
                });
            var result = codes.OrderByDescending(p => p.DiscountPercent);

            if (isRedisEnabled)
                await _cacheService.SetAsync(ActiveCodesKey, result, TimeSpan.FromMinutes(15));

            return Result<IEnumerable<PromoCodeDto>>.Ok(result);
        }
    }
}
