using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Cache;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.PromoCode.Commands.IncrementPromoCodeUsage
{
    public class IncrementPromoCodeUsageCommandHandler : IRequestHandler<IncrementPromoCodeUsageCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICacheService _cacheService;

        private const string ActiveCodesKey = "promos:active";

        public IncrementPromoCodeUsageCommandHandler(IApplicationDbContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<Result<string>> Handle(IncrementPromoCodeUsageCommand request, CancellationToken cancellationToken)
        {
            var promo = await _context.PromoCodes.FindAsync(request.PromoCodeId);
            if (promo == null)
                return Result<string>.Fail("Promo code not found.");

            promo.RecordUsage();
            _context.PromoCodes.Update(promo);
            await _context.SaveChangesAsync(cancellationToken);

            var redis = await _context.IntegrationSettings.Where(s => s.ServiceName == "Redis")
                .Select(s => new { s.IsEnabled }).FirstOrDefaultAsync(cancellationToken);
            if (redis != null && redis.IsEnabled)
                await _cacheService.RemoveAsync(ActiveCodesKey);

            return Result<string>.Ok(null, "Usage incremented.");
        }
    }
}
