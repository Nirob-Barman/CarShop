using CarShop.Application.Interfaces;
using CarShop.Application.DTOs.Payment;
using CarShop.Application.Mappers;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.PaymentGateway.Queries.GetActiveGateways
{
    public class GetActiveGatewaysQueryHandler : IRequestHandler<GetActiveGatewaysQuery, Result<IEnumerable<PaymentGatewayDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetActiveGatewaysQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IEnumerable<PaymentGatewayDto>>> Handle(GetActiveGatewaysQuery request, CancellationToken cancellationToken)
        {
            var gateways = await _context.PaymentGateways.AsNoTracking()
                .Where(g => g.IsActive)
                .Select(g => PaymentGatewayMapper.ToDto(g))
                .ToListAsync(cancellationToken);
            return Result<IEnumerable<PaymentGatewayDto>>.Ok(gateways.OrderBy(g => g.SortOrder));
        }
    }
}
