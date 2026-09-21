using CarShop.Application.Interfaces;
using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.PromoCode.Queries.GetAllPromoCodes
{
    public class GetAllPromoCodesQueryHandler : IRequestHandler<GetAllPromoCodesQuery, Result<IEnumerable<PromoCodeDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllPromoCodesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IEnumerable<PromoCodeDto>>> Handle(GetAllPromoCodesQuery request, CancellationToken cancellationToken)
        {
            var codes = await _context.PromoCodes.ToListAsync();
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
