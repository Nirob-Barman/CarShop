using CarShop.Application.Interfaces;
using Stripe.Checkout;

namespace CarShop.Infrastructure.Payments
{
    public class StripePaymentProcessor : IPaymentProcessor
    {
        public string GatewaySlug => "stripe";

        public async Task<PaymentInitResult> InitiateAsync(PaymentRequest request)
        {
            try
            {
                if (!request.Config.TryGetValue("secret_key", out var secretKey) || string.IsNullOrEmpty(secretKey))
                    return new PaymentInitResult(false, null, null, "Stripe secret key not configured.");

                Stripe.StripeConfiguration.ApiKey = secretKey;

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = ["card"],
                    Mode = "payment",
                    LineItems =
                    [
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency   = request.Currency.ToLower(),
                                UnitAmount = (long)(request.Amount * 100),
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name        = request.CarTitle,
                                    Description = $"Order #{request.OrderId} — CarShop"
                                }
                            },
                            Quantity = 1
                        }
                    ],
                    SuccessUrl = request.SuccessUrl,
                    CancelUrl  = request.CancelUrl,
                    Metadata   = new Dictionary<string, string>
                    {
                        { "orderId",       request.OrderId.ToString() },
                        { "transactionId", request.TransactionDbId.ToString() }
                    }
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options);
                return new PaymentInitResult(true, session.Url, session.Id, null);
            }
            catch (Exception ex)
            {
                return new PaymentInitResult(false, null, null, ex.Message);
            }
        }

        public async Task<PaymentVerifyResult> VerifyAsync(string sessionRef, Dictionary<string, string> config)
        {
            try
            {
                if (!config.TryGetValue("secret_key", out var secretKey) || string.IsNullOrEmpty(secretKey))
                    return new PaymentVerifyResult(false, null, null, "Stripe secret key not configured.");

                Stripe.StripeConfiguration.ApiKey = secretKey;
                var service = new SessionService();
                var session = await service.GetAsync(sessionRef);
                var success = session.PaymentStatus == "paid";
                return new PaymentVerifyResult(success, session.PaymentIntentId,
                    $"{{\"status\":\"{session.PaymentStatus}\",\"session_id\":\"{session.Id}\"}}",
                    success ? null : "Payment not completed.");
            }
            catch (Exception ex)
            {
                return new PaymentVerifyResult(false, null, null, ex.Message);
            }
        }
    }
}
