using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PaymentGateway.Commands.ToggleGatewayActive
{
    public record ToggleGatewayActiveCommand(int Id)
        : IRequest<Result<string>>;
}
