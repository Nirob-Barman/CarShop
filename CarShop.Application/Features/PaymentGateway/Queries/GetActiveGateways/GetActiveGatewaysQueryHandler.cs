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
                .OrderBy(g => g.SortOrder)
                .ToListAsync(cancellationToken);

            var result = gateways.Select(PaymentGatewayMapper.ToDto);

            return Result<IEnumerable<PaymentGatewayDto>>.Ok(result);
        }
    }
}
