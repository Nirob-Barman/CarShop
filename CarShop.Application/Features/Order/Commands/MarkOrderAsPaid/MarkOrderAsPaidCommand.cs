using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Commands.MarkOrderAsPaid
{
    public class MarkOrderAsPaidCommand : IRequest<Result<string>>
    {
        public int OrderId { get; set; }

        public MarkOrderAsPaidCommand(int orderId)
        {
            OrderId = orderId;
        }
    }
}
