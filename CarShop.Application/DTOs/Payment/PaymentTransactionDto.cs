namespace CarShop.Application.DTOs.Payment
{
    public class PaymentTransactionDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int PaymentGatewayId { get; set; }
        public string? GatewayName { get; set; }
        public string? GatewaySlug { get; set; }
        public string? TransactionId { get; set; }
        public string? SessionRef { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
    }
}
