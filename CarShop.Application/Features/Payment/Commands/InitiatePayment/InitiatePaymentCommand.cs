using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Payment.Commands.InitiatePayment
{
    public record InitiatePaymentCommand(
        int CarId,
        int GatewayId,
        string? PromoCode,
        string SuccessUrl,
        string CancelUrl)
        : IRequest<Result<string>>;
}
