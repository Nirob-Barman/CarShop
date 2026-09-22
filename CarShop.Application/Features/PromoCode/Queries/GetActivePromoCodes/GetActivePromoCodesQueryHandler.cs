using CarShop.Application.Interfaces;
using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.PromoCode.Queries.GetActivePromoCodes
{
    public class GetActivePromoCodesQueryHandler : IRequestHandler<GetActivePromoCodesQuery, Result<IEnumerable<PromoCodeDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetActivePromoCodesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IEnumerable<PromoCodeDto>>> Handle(GetActivePromoCodesQuery request, CancellationToken cancellationToken)
        {
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

            return Result<IEnumerable<PromoCodeDto>>.Ok(result);
        }
    }
}
