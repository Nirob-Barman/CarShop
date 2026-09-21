using CarShop.Application.Interfaces;
using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Queries.GetPromoCodeById
{
    public class GetPromoCodeByIdQueryHandler : IRequestHandler<GetPromoCodeByIdQuery, Result<PromoCodeDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetPromoCodeByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PromoCodeDto>> Handle(GetPromoCodeByIdQuery request, CancellationToken cancellationToken)
        {
            var promo = await _context.PromoCodes.FindAsync(request.Id);
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
