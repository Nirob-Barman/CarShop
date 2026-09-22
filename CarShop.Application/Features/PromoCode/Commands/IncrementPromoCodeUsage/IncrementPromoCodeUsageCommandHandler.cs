using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Commands.IncrementPromoCodeUsage
{
    public class IncrementPromoCodeUsageCommandHandler : IRequestHandler<IncrementPromoCodeUsageCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;

        public IncrementPromoCodeUsageCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<string>> Handle(IncrementPromoCodeUsageCommand request, CancellationToken cancellationToken)
        {
            var promo = await _context.PromoCodes.FindAsync(request.PromoCodeId);
            if (promo == null)
                return Result<string>.Fail("Promo code not found.");

            promo.RecordUsage();
            _context.PromoCodes.Update(promo);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "Usage incremented.");
        }
    }
}
