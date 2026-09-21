using CarShop.Application.Interfaces;
using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Interfaces.Cache;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.PromoCode.Queries.GetActivePromoCodes
{
    public class GetActivePromoCodesQueryHandler : IRequestHandler<GetActivePromoCodesQuery, Result<IEnumerable<PromoCodeDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICacheService _cacheService;

        private const string ActiveCodesKey = "promos:active";

        public GetActivePromoCodesQueryHandler(IApplicationDbContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<Result<IEnumerable<PromoCodeDto>>> Handle(GetActivePromoCodesQuery request, CancellationToken cancellationToken)
        {
            var redis = await _context.IntegrationSettings.AsNoTracking().Where(s => s.ServiceName == "Redis")
                .Select(s => new { s.IsEnabled }).FirstOrDefaultAsync(cancellationToken);
            var isRedisEnabled = redis != null && redis.IsEnabled;

            if (isRedisEnabled)
            {
                var cached = await _cacheService.GetAsync<IEnumerable<PromoCodeDto>>(ActiveCodesKey);
                if (cached != null)
                    return Result<IEnumerable<PromoCodeDto>>.Ok(cached);
            }

            var now   = DateTime.UtcNow;
            var codes = await _context.PromoCodes.AsNoTracking().Where(p => p.IsActive &&
                     (!p.MaxUsages.HasValue || p.UsageCount < p.MaxUsages.Value) &&
                     (!p.ExpiresAt.HasValue || p.ExpiresAt.Value > now))
                .Select(p => new PromoCodeDto
                {
                    Id                = p.Id,
                    Code              = p.Code,
                    DiscountPercent   = p.DiscountPercent,
                    MaxDiscountAmount = p.MaxDiscountAmount,
                    MaxUsages         = p.MaxUsages,
                    UsageCount        = p.UsageCount,
                    ExpiresAt         = p.ExpiresAt,
                    IsActive          = p.IsActive
                }).ToListAsync(cancellationToken);
            var result = codes.OrderByDescending(p => p.DiscountPercent);

            if (isRedisEnabled)
                await _cacheService.SetAsync(ActiveCodesKey, result, TimeSpan.FromMinutes(15));

            return Result<IEnumerable<PromoCodeDto>>.Ok(result);
        }
    }
}
