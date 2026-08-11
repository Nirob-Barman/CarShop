namespace CarShop.Domain.Entities
{
    public class PaymentGateway : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string GatewayFamily { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Type { get; set; } = "online";
        public string? LogoUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsSandbox { get; set; } = true;
        public string SupportedCurrencies { get; set; } = "USD";
        public string? Config { get; set; }
        public int SortOrder { get; set; } = 0;

        public ICollection<PaymentTransaction> Transactions { get; set; } = [];
        public ICollection<Order> Orders { get; set; } = [];
    }
}
