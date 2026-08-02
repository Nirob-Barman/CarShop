using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Commands.ExpireStalePendingOrders
{
    public class ExpireStalePendingOrdersCommand : IRequest<Result<string>>
    {
        public int OlderThanMinutes { get; set; }

        public ExpireStalePendingOrdersCommand(int olderThanMinutes = 30)
        {
            OlderThanMinutes = olderThanMinutes;
        }
    }
}
