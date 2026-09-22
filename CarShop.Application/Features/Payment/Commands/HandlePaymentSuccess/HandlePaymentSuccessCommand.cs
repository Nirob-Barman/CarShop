using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Payment.Commands.HandlePaymentSuccess
{
    public record HandlePaymentSuccessCommand(
        int TransactionDbId,
        string GatewaySlug,
        string? SessionRefOverride = null)
        : IRequest<Result<string>>;
}
