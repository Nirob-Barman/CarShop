using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Commands.SetOrderGateway
{
    public class SetOrderGatewayCommand : IRequest<Result<string>>
    {
        public int OrderId { get; set; }
        public int PaymentGatewayId { get; set; }

        public SetOrderGatewayCommand(int orderId, int paymentGatewayId)
        {
            OrderId = orderId;
            PaymentGatewayId = paymentGatewayId;
        }
    }
}
