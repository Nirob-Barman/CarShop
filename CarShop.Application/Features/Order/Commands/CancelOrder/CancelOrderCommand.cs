using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Commands.CancelOrder
{
    public class CancelOrderCommand : IRequest<Result<string>>
    {
        public int OrderId { get; set; }

        public CancelOrderCommand(int orderId)
        {
            OrderId = orderId;
        }
    }
}
