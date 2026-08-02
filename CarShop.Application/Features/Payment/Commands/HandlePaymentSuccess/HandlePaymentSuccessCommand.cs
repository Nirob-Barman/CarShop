using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Payment.Commands.HandlePaymentSuccess
{
    public class HandlePaymentSuccessCommand : IRequest<Result<string>>
    {
        public int TransactionDbId { get; set; }
        public string GatewaySlug { get; set; }
        public string? SessionRefOverride { get; set; }

        public HandlePaymentSuccessCommand(int transactionDbId, string gatewaySlug, string? sessionRefOverride = null)
        {
            TransactionDbId = transactionDbId;
            GatewaySlug = gatewaySlug;
            SessionRefOverride = sessionRefOverride;
        }
    }
}
