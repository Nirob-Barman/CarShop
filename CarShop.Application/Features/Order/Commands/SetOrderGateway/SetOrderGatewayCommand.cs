using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Commands.SetOrderGateway
{
    public record SetOrderGatewayCommand(int OrderId, int PaymentGatewayId)
        : IRequest<Result<string>>;
}
