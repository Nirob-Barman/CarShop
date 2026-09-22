using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Commands.CancelPendingOrderById
{
    public record CancelPendingOrderByIdCommand(int OrderId)
        : IRequest<Result<string>>;
}
