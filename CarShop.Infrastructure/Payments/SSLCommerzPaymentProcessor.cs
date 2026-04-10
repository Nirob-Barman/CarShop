using CarShop.Application.Interfaces;
using System.Text.Json;

namespace CarShop.Infrastructure.Payments
{
    public class SSLCommerzPaymentProcessor : IPaymentProcessor
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SSLCommerzPaymentProcessor(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public string GatewaySlug => "sslcommerz";

        public async Task<PaymentInitResult> InitiateAsync(PaymentRequest request)
        {
            if (!request.Config.TryGetValue("store_id", out var storeId) || string.IsNullOrEmpty(storeId))
                return new PaymentInitResult(false, null, null, "SSLCommerz store_id not configured.");

            if (!request.Config.TryGetValue("store_password", out var storePassword) || string.IsNullOrEmpty(storePassword))
                return new PaymentInitResult(false, null, null, "SSLCommerz store_password not configured.");

            bool isSandbox = request.Config.TryGetValue("_is_sandbox", out var sb) && sb == "true";
            var apiUrl = isSandbox
                ? "https://sandbox.sslcommerz.com/gwprocess/v4/api.php"
                : "https://securepay.sslcommerz.com/gwprocess/v4/api.php";

            // Unique tran_id scoped to this transaction row
            var tranId = $"CS_{request.TransactionDbId}_{DateTime.UtcNow:yyyyMMddHHmmss}";

            var form = new Dictionary<string, string>
            {
                ["store_id"]         = storeId,
                ["store_passwd"]     = storePassword,
                ["total_amount"]     = request.Amount.ToString("F2"),
                ["currency"]         = request.Currency,
                ["tran_id"]          = tranId,
                ["success_url"]      = request.SuccessUrl,
                ["fail_url"]         = request.CancelUrl,
                ["cancel_url"]       = request.CancelUrl,
                ["cus_name"]         = "Customer",
                ["cus_email"]        = "customer@carshop.com",
                ["cus_add1"]         = "N/A",
                ["cus_city"]         = "Dhaka",
                ["cus_state"]        = "Dhaka",
                ["cus_postcode"]     = "1200",
                ["cus_country"]      = "Bangladesh",
                ["cus_phone"]        = "01700000000",
                ["ship_name"]        = "Customer",
                ["ship_add1"]        = "N/A",
                ["ship_city"]        = "Dhaka",
                ["ship_state"]       = "Dhaka",
                ["ship_postcode"]    = "1200",
                ["ship_country"]     = "Bangladesh",
                ["product_name"]     = request.CarTitle,
                ["product_category"] = "Automobile",
                ["product_profile"]  = "physical-goods",
            };

            try
            {
                var client   = _httpClientFactory.CreateClient();
                var response = await client.PostAsync(apiUrl, new FormUrlEncodedContent(form));
                var json     = await response.Content.ReadAsStringAsync();

                using var doc  = JsonDocument.Parse(json);
                var root       = doc.RootElement;
                var status     = root.GetProperty("status").GetString();

                if (status != "SUCCESS")
                {
                    var reason = root.TryGetProperty("failedreason", out var fr) ? fr.GetString() : "SSLCommerz initiation failed.";
                    return new PaymentInitResult(false, null, null, reason);
                }

                var gatewayUrl = root.GetProperty("GatewayPageURL").GetString();
                return new PaymentInitResult(true, gatewayUrl, tranId, null);
            }
            catch (Exception ex)
            {
                return new PaymentInitResult(false, null, null, ex.Message);
            }
        }

        public async Task<PaymentVerifyResult> VerifyAsync(string sessionRef, Dictionary<string, string> config)
        {
            // sessionRef = val_id supplied by SSLCommerz in the success callback POST body
            if (!config.TryGetValue("store_id", out var storeId) || string.IsNullOrEmpty(storeId))
                return new PaymentVerifyResult(false, null, null, "SSLCommerz store_id not configured.");

            if (!config.TryGetValue("store_password", out var storePassword) || string.IsNullOrEmpty(storePassword))
                return new PaymentVerifyResult(false, null, null, "SSLCommerz store_password not configured.");

            bool isSandbox = config.TryGetValue("_is_sandbox", out var sb) && sb == "true";
            var validationBase = isSandbox
                ? "https://sandbox.sslcommerz.com/validator/api/validationserverAPI.php"
                : "https://securepay.sslcommerz.com/validator/api/validationserverAPI.php";

            var url = $"{validationBase}" +
                      $"?val_id={Uri.EscapeDataString(sessionRef)}" +
                      $"&store_id={Uri.EscapeDataString(storeId)}" +
                      $"&store_passwd={Uri.EscapeDataString(storePassword)}" +
                      $"&v=1&format=json";

            try
            {
                var client   = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(url);
                var json     = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(json);
                var root      = doc.RootElement;
                var status    = root.GetProperty("status").GetString();
                var isValid   = status == "VALID" || status == "VALIDATED";

                string? tranId = root.TryGetProperty("tran_id", out var tid) ? tid.GetString() : null;

                return new PaymentVerifyResult(isValid, tranId, json,
                    isValid ? null : $"SSLCommerz validation status: {status}");
            }
            catch (Exception ex)
            {
                return new PaymentVerifyResult(false, null, null, ex.Message);
            }
        }
    }
}
