using CarShop.Application.DTOs.Payment;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PaymentGateway.Queries.GetAllGateways
{
    public class GetAllGatewaysQuery : IRequest<Result<IEnumerable<PaymentGatewayDto>>>
    {
    }
}
