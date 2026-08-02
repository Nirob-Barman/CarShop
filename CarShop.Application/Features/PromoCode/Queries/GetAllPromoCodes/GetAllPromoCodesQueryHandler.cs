using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using PromoCodeEntity = CarShop.Domain.Entities.PromoCode;

namespace CarShop.Application.Features.PromoCode.Queries.GetAllPromoCodes
{
    public class GetAllPromoCodesQueryHandler : IRequestHandler<GetAllPromoCodesQuery, Result<IEnumerable<PromoCodeDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllPromoCodesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<PromoCodeDto>>> Handle(GetAllPromoCodesQuery request, CancellationToken cancellationToken)
        {
            var codes = await _unitOfWork.Repository<PromoCodeEntity>().GetAllAsync();
            var dtos = codes.Select(p => new PromoCodeDto
            {
                Id = p.Id,
                Code = p.Code,
                DiscountPercent = p.DiscountPercent,
                MaxDiscountAmount = p.MaxDiscountAmount,
                MaxUsages = p.MaxUsages,
                UsageCount = p.UsageCount,
                ExpiresAt = p.ExpiresAt,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            });
            return Result<IEnumerable<PromoCodeDto>>.Ok(dtos);
        }
    }
}
