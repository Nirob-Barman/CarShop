using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using PromoCodeEntity = CarShop.Domain.Entities.PromoCode;

namespace CarShop.Application.Features.PromoCode.Queries.GetPromoCodeById
{
    public class GetPromoCodeByIdQueryHandler : IRequestHandler<GetPromoCodeByIdQuery, Result<PromoCodeDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPromoCodeByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PromoCodeDto>> Handle(GetPromoCodeByIdQuery request, CancellationToken cancellationToken)
        {
            var promo = await _unitOfWork.Repository<PromoCodeEntity>().GetByIdAsync(request.Id);
            if (promo == null) return Result<PromoCodeDto>.Fail("Promo code not found.");
            return Result<PromoCodeDto>.Ok(new PromoCodeDto
            {
                Id = promo.Id, Code = promo.Code, DiscountPercent = promo.DiscountPercent,
                MaxDiscountAmount = promo.MaxDiscountAmount, MaxUsages = promo.MaxUsages,
                UsageCount = promo.UsageCount, ExpiresAt = promo.ExpiresAt,
                IsActive = promo.IsActive, CreatedAt = promo.CreatedAt
            });
        }
    }
}
