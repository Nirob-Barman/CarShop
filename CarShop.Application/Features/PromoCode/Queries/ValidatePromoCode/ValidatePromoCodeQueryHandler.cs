using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using PromoCodeEntity = CarShop.Domain.Entities.PromoCode;

namespace CarShop.Application.Features.PromoCode.Queries.ValidatePromoCode
{
    public class ValidatePromoCodeQueryHandler : IRequestHandler<ValidatePromoCodeQuery, Result<ValidatePromoCodeResult>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ValidatePromoCodeQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ValidatePromoCodeResult>> Handle(ValidatePromoCodeQuery request, CancellationToken cancellationToken)
        {
            var promo = await _unitOfWork.Repository<PromoCodeEntity>().FirstOrDefaultAsync(
                p => p.Code == request.Code.ToUpper() && p.IsActive);

            if (promo == null)
                return Result<ValidatePromoCodeResult>.Fail("Invalid or inactive promo code.");

            if (promo.ExpiresAt.HasValue && promo.ExpiresAt.Value < DateTime.UtcNow)
                return Result<ValidatePromoCodeResult>.Fail("Promo code has expired.");

            if (promo.MaxUsages.HasValue && promo.UsageCount >= promo.MaxUsages.Value)
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
