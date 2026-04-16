namespace CarShop.Application.Payment
{
    public class GatewayFieldDefinition
    {
        public string Key { get; init; } = string.Empty;
        public string Label { get; init; } = string.Empty;
        public bool IsSecret { get; init; }
        public bool IsRequired { get; init; } = true;
        public string? Placeholder { get; init; }
    }

    public class GatewayDefinition
    {
        public string Slug { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
        public string VariantLabel { get; init; } = string.Empty;
        public List<GatewayFieldDefinition> Fields { get; init; } = new();
    }

    public class GatewayFamily
    {
        public string Key { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
        public List<GatewayDefinition> Variants { get; init; } = new();
        public bool HasVariants => Variants.Count > 1;
    }

    public static class GatewayConfigSchema
    {
        private static readonly List<GatewayFieldDefinition> StripeBaseFields =
        [
            new() { Key = "secret_key", Label = "Secret Key", IsSecret = true, Placeholder = "sk_test_..." },
            new() { Key = "publishable_key", Label = "Publishable Key", IsSecret = false, Placeholder = "pk_test_..." },
            new() { Key = "webhook_secret", Label = "Webhook Secret", IsSecret = true, IsRequired = false, Placeholder = "whsec_..." }
        ];

        private static readonly List<GatewayFieldDefinition> SslBaseFields =
        [
            new() { Key = "store_id", Label = "Store ID", IsSecret = false, Placeholder = "your_store_id" },
            new() { Key = "store_password", Label = "Store Password", IsSecret = true, Placeholder = "store_password" }
        ];

        private static readonly List<GatewayFieldDefinition> BkashBaseFields =
        [
            new() { Key = "app_key", Label = "App Key", IsSecret = false, Placeholder = "app_key" },
            new() { Key = "app_secret", Label = "App Secret", IsSecret = true, Placeholder = "app_secret" },
            new() { Key = "username", Label = "Username", IsSecret = false, Placeholder = "username" },
            new() { Key = "password", Label = "Password", IsSecret = true, Placeholder = "password" }
        ];

        private static readonly List<GatewayFieldDefinition> SurjoBaseFields =
        [
            new() { Key = "username", Label = "Username", IsSecret = false, Placeholder = "username" },
            new() { Key = "password", Label = "Password", IsSecret = true, Placeholder = "password" }
        ];

        public static readonly List<GatewayFamily> Families =
        [
            new()
            {
                Key = "stripe",
                DisplayName = "Stripe",
                Variants =
                [
                    new()
                    {
                        Slug = "stripe_checkout",
                        DisplayName = "Stripe Checkout",
                        VariantLabel = "Checkout",
                        Fields = new(StripeBaseFields)
                    },
                    new()
                    {
                        Slug = "stripe_payment_intents",
                        DisplayName = "Stripe Payment Intents",
                        VariantLabel = "Payment Intents",
                        Fields = new(StripeBaseFields)
                    }
                ]
            },
            new()
            {
                Key = "sslcommerz",
                DisplayName = "SSLCommerz",
                Variants =
                [
                    new()
                    {
                        Slug = "sslcommerz_hosted",
                        DisplayName = "SSLCommerz Hosted",
                        VariantLabel = "Hosted Payment",
                        Fields = new(SslBaseFields)
                    },
                    new()
                    {
                        Slug = "sslcommerz_easy",
                        DisplayName = "SSLCommerz Easy Checkout",
                        VariantLabel = "Easy Checkout",
                        Fields = new(SslBaseFields)
                    }
                ]
            },
            new()
            {
                Key = "bkash",
                DisplayName = "bKash",
                Variants =
                [
                    new()
                    {
                        Slug = "bkash_checkout",
                        DisplayName = "bKash Checkout",
                        VariantLabel = "Checkout",
                        Fields = new(BkashBaseFields)
                    },
                    new()
                    {
                        Slug = "bkash_tokenized",
                        DisplayName = "bKash Tokenized",
                        VariantLabel = "Tokenized",
                        Fields = new(BkashBaseFields)
                    },
                    new()
                    {
                        Slug = "bkash_webhook",
                        DisplayName = "bKash Webhook",
                        VariantLabel = "Webhook",
                        Fields =
                        [
                            .. BkashBaseFields,
                            new() { Key = "webhook_secret", Label = "Webhook Secret", IsSecret = true, IsRequired = false }
                        ]
                    }
                ]
            },
            new()
            {
                Key = "surjopay",
                DisplayName = "SurjoPay",
                Variants =
                [
                    new()
                    {
                        Slug = "surjopay_checkout",
                        DisplayName = "SurjoPay Checkout",
                        VariantLabel = "Checkout",
                        Fields = new(SurjoBaseFields)
                    },
                    new()
                    {
                        Slug = "surjopay_seamless",
                        DisplayName = "SurjoPay Seamless",
                        VariantLabel = "Seamless",
                        Fields = new(SurjoBaseFields)
                    }
                ]
            }
        ];

        public static IReadOnlyList<GatewayDefinition> All =>
            Families.SelectMany(f => f.Variants).ToList();

        public static IReadOnlyDictionary<string, List<GatewayFieldDefinition>> Fields =>
            All.ToDictionary(g => g.Slug, g => g.Fields, StringComparer.OrdinalIgnoreCase);

        public static readonly string[] KnownSlugs = All.Select(g => g.Slug).ToArray();

        public static GatewayDefinition? Get(string slug)
        {
            var gateway = All.FirstOrDefault(g => string.Equals(g.Slug, slug, StringComparison.OrdinalIgnoreCase));
            if (gateway != null)
            {
                return gateway;
            }

            var family = GetFamily(slug);
            var fallback = family?.Variants.FirstOrDefault();
            if (family == null || fallback == null)
            {
                return null;
            }

            return new GatewayDefinition
            {
                Slug = slug,
                DisplayName = family.DisplayName,
                VariantLabel = family.HasVariants ? "Legacy" : family.DisplayName,
                Fields = new(fallback.Fields)
            };
        }

        public static GatewayFamily? GetFamily(string slug) =>
            Families.FirstOrDefault(f =>
                string.Equals(f.Key, slug, StringComparison.OrdinalIgnoreCase) ||
                f.Variants.Any(v => string.Equals(v.Slug, slug, StringComparison.OrdinalIgnoreCase)));

        public static string GetFamilyKey(string slug) =>
            GetFamily(slug)?.Key ?? slug.Split('_', 2)[0].ToLowerInvariant();

        public static List<GatewayFieldDefinition> GetFields(string slug) =>
            Get(slug)?.Fields ?? [];
    }
}
