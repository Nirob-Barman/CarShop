using CarShop.Application.Interfaces;

namespace CarShop.Infrastructure.Payments
{
    public class SurjoPayPaymentProcessor : IPaymentProcessor
    {
        public string GatewaySlug => "surjopay";

        public Task<PaymentInitResult> InitiateAsync(PaymentRequest request)
        {
            // TODO: Implement SurjoPay payment initiation
            // Requires: username, password from config
            return Task.FromResult(new PaymentInitResult(false, null, null,
                "SurjoPay integration is not yet implemented. Please contact support."));
        }

        public Task<PaymentVerifyResult> VerifyAsync(string sessionRef, Dictionary<string, string> config)
        {
            return Task.FromResult(new PaymentVerifyResult(false, null, null,
                "SurjoPay verification not yet implemented."));
        }
    }
}
