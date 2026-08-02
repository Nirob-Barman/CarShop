using CarShop.Application.DTOs.Payment;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PaymentGateway.Queries.GetActiveGateways
{
    public class GetActiveGatewaysQuery : IRequest<Result<IEnumerable<PaymentGatewayDto>>>
    {
    }
}
