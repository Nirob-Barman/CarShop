using CarShop.Application.Interfaces;
using CarShop.Application.DTOs.Payment;
using CarShop.Application.Mappers;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PaymentGateway.Queries.GetGatewayById
{
    public class GetGatewayByIdQueryHandler : IRequestHandler<GetGatewayByIdQuery, Result<PaymentGatewayDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetGatewayByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaymentGatewayDto>> Handle(GetGatewayByIdQuery request, CancellationToken cancellationToken)
        {
            var gateway = await _context.PaymentGateways.FindAsync(request.Id);
            if (gateway == null) return Result<PaymentGatewayDto>.Fail("Gateway not found.");
            return Result<PaymentGatewayDto>.Ok(PaymentGatewayMapper.ToDto(gateway));
        }
    }
}
