using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Commands.ExpireStalePendingOrders
{
    public record ExpireStalePendingOrdersCommand(int OlderThanMinutes = 30)
        : IRequest<Result<string>>;
}
