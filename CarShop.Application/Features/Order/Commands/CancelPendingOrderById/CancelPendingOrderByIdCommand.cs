using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Commands.CancelPendingOrderById
{
    public class CancelPendingOrderByIdCommand : IRequest<Result<string>>
    {
        public int OrderId { get; set; }

        public CancelPendingOrderByIdCommand(int orderId)
        {
            OrderId = orderId;
        }
    }
}
