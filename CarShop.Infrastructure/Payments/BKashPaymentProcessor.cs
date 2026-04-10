using CarShop.Application.Interfaces;

namespace CarShop.Infrastructure.Payments
{
    public class BKashPaymentProcessor : IPaymentProcessor
    {
        public string GatewaySlug => "bkash";

        public Task<PaymentInitResult> InitiateAsync(PaymentRequest request)
        {
            // TODO: Implement bKash payment initiation
            // Requires: app_key, app_secret, username, password from config
            return Task.FromResult(new PaymentInitResult(false, null, null,
                "bKash integration is not yet implemented. Please contact support."));
        }

        public Task<PaymentVerifyResult> VerifyAsync(string sessionRef, Dictionary<string, string> config)
        {
            return Task.FromResult(new PaymentVerifyResult(false, null, null,
                "bKash verification not yet implemented."));
        }
    }
}
