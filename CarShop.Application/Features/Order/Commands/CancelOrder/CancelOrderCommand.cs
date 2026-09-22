using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Commands.CancelOrder
{
    public record CancelOrderCommand(int OrderId)
        : IRequest<Result<string>>;
}
