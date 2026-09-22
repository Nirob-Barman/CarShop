using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Commands.AdminCancelOrder
{
    public record AdminCancelOrderCommand(int OrderId)
        : IRequest<Result<string>>;
}
