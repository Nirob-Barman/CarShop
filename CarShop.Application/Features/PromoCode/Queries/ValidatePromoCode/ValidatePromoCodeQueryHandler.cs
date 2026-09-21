using CarShop.Application.Interfaces;
using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.PromoCode.Queries.ValidatePromoCode
{
    public class ValidatePromoCodeQueryHandler : IRequestHandler<ValidatePromoCodeQuery, Result<ValidatePromoCodeResult>>
    {
        private readonly IApplicationDbContext _context;

        public ValidatePromoCodeQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ValidatePromoCodeResult>> Handle(ValidatePromoCodeQuery request, CancellationToken cancellationToken)
        {
            var promo = await _context.PromoCodes.AsNoTracking().FirstOrDefaultAsync(
                p => p.Code == request.Code.ToUpper() && p.IsActive);

            if (promo == null)
                return Result<ValidatePromoCodeResult>.Fail("Invalid or inactive promo code.");

            if (promo.IsExpired)
                return Result<ValidatePromoCodeResult>.Fail("Promo code has expired.");

            if (promo.HasReachedUsageLimit)
                return Result<ValidatePromoCodeResult>.Fail("Promo code usage limit reached.");

            return Result<ValidatePromoCodeResult>.Ok(new ValidatePromoCodeResult
            {
                IsValid = true,
                DiscountPercent = promo.DiscountPercent,
                MaxDiscountAmount = promo.MaxDiscountAmount,
                PromoCodeId = promo.Id,
                Message = $"{promo.DiscountPercent}% discount applied!"
            });
        }
    }
}
