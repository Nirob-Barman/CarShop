namespace CarShop.Application.DTOs.Payment
{
    public class PaymentGatewayDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string GatewayFamily { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Type { get; set; } = "online";
        public string? LogoUrl { get; set; }
        public bool IsActive { get; set; }
        public bool IsSandbox { get; set; }
        public string SupportedCurrencies { get; set; } = "USD";
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
