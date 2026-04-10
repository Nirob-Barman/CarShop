namespace CarShop.Application.Payment
{
    public static class GatewayConfigSchema
    {
        public record ConfigField(string Key, string Label, bool IsSecret, string Placeholder = "");

        public static readonly Dictionary<string, List<ConfigField>> Fields = new()
        {
            ["stripe"] = [
                new("publishable_key", "Publishable Key", false, "pk_test_..."),
                new("secret_key", "Secret Key", true, "sk_test_...")
            ],
            ["sslcommerz"] = [
                new("store_id", "Store ID", false, "your_store_id"),
                new("store_password", "Store Password", true, "store_password")
            ],
            ["bkash"] = [
                new("app_key", "App Key", false, "app_key"),
                new("app_secret", "App Secret", true, "app_secret"),
                new("username", "Username", false, "username"),
                new("password", "Password", true, "password")
            ],
            ["surjopay"] = [
                new("username", "Username", false, "username"),
                new("password", "Password", true, "password")
            ]
        };

        public static readonly string[] KnownSlugs = ["stripe", "sslcommerz", "bkash", "surjopay"];

        public static List<ConfigField> GetFields(string slug) =>
            Fields.TryGetValue(slug.ToLower(), out var fields) ? fields : [];
    }
}
