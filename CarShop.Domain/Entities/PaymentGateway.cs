namespace CarShop.Domain.Entities
{
    public class PaymentGateway : BaseAuditableEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string GatewayFamily { get; private set; } = string.Empty;
        public string Slug { get; private set; } = string.Empty;
        public string Type { get; private set; } = "online";
        public string? LogoUrl { get; private set; }
        public bool IsActive { get; private set; } = true;
        public bool IsSandbox { get; private set; } = true;
        public string SupportedCurrencies { get; private set; } = "USD";
        public string? Config { get; private set; }
        public int SortOrder { get; private set; } = 0;

        public ICollection<PaymentTransaction> Transactions { get; private set; } = [];
        public ICollection<Order> Orders { get; private set; } = [];

        private PaymentGateway(
            string name,
            string gatewayFamily,
            string slug,
            string type,
            string? logoUrl,
            bool isActive,
            bool isSandbox,
            string supportedCurrencies,
            int sortOrder,
            string? config)
        {
            SetName(name);
            SetGatewayFamily(gatewayFamily);
            SetSlug(slug);
            SetType(type);
            SetLogoUrl(logoUrl);
            SetSupportedCurrencies(supportedCurrencies);
            SetSortOrder(sortOrder);

            IsActive = isActive;
            IsSandbox = isSandbox;
            Config = config;

            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public static PaymentGateway Create(
            string name,
            string gatewayFamily,
            string slug,
            string type = "online",
            string? logoUrl = null,
            bool isActive = true,
            bool isSandbox = true,
            string supportedCurrencies = "USD",
            int sortOrder = 0,
            string? config = null)
        {
            return new PaymentGateway(
                name,
                gatewayFamily,
                slug,
                type,
                logoUrl,
                isActive,
                isSandbox,
                supportedCurrencies,
                sortOrder,
                config);
        }

        public void Update(
            string name,
            string gatewayFamily,
            string type,
            string? logoUrl,
            bool isActive,
            bool isSandbox,
            string supportedCurrencies,
            int sortOrder)
        {
            SetName(name);
            SetGatewayFamily(gatewayFamily);
            SetType(type);
            SetLogoUrl(logoUrl);
            SetSupportedCurrencies(supportedCurrencies);
            SetSortOrder(sortOrder);

            IsActive = isActive;
            IsSandbox = isSandbox;

            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateConfig(string? config)
        {
            Config = config;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ToggleActive()
        {
            IsActive = !IsActive;
            UpdatedAt = DateTime.UtcNow;
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Gateway name is required.", nameof(name));

            Name = name.Trim();
        }

        private void SetGatewayFamily(string gatewayFamily)
        {
            if (string.IsNullOrWhiteSpace(gatewayFamily))
                throw new ArgumentException("Gateway family is required.", nameof(gatewayFamily));

            GatewayFamily = gatewayFamily.Trim().ToLowerInvariant();
        }

        private void SetSlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Gateway slug is required.", nameof(slug));

            Slug = slug.Trim().ToLowerInvariant();
        }

        private void SetType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Gateway type is required.", nameof(type));

            Type = type.Trim().ToLowerInvariant();
        }

        private void SetLogoUrl(string? logoUrl)
        {
            LogoUrl = string.IsNullOrWhiteSpace(logoUrl) ? null : logoUrl.Trim();
        }

        private void SetSupportedCurrencies(string supportedCurrencies)
        {
            if (string.IsNullOrWhiteSpace(supportedCurrencies))
                throw new ArgumentException("Supported currencies are required.", nameof(supportedCurrencies));

            SupportedCurrencies = supportedCurrencies.Trim().ToUpperInvariant();
        }

        private void SetSortOrder(int sortOrder)
        {
            if (sortOrder < 0)
                throw new ArgumentOutOfRangeException(nameof(sortOrder), "Sort order cannot be negative.");

            SortOrder = sortOrder;
        }
    }
}
