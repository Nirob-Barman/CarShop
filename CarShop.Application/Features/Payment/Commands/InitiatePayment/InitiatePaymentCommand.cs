using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Payment.Commands.InitiatePayment
{
    public class InitiatePaymentCommand : IRequest<Result<string>>
    {
        public int CarId { get; set; }
        public int GatewayId { get; set; }
        public string? PromoCode { get; set; }
        public string SuccessUrl { get; set; }
        public string CancelUrl { get; set; }

        public InitiatePaymentCommand(int carId, int gatewayId, string? promoCode, string successUrl, string cancelUrl)
        {
            CarId = carId;
            GatewayId = gatewayId;
            PromoCode = promoCode;
            SuccessUrl = successUrl;
            CancelUrl = cancelUrl;
        }
    }
}
