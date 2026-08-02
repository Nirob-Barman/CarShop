using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Commands.AdminCancelOrder
{
    public class AdminCancelOrderCommand : IRequest<Result<string>>
    {
        public int OrderId { get; set; }

        public AdminCancelOrderCommand(int orderId)
        {
            OrderId = orderId;
        }
    }
}
