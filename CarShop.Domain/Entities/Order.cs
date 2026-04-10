namespace CarShop.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int CarId { get; set; }
        public DateTime OrderedAt { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; } = "Confirmed";
        public string? PromoCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
        public int? PaymentGatewayId { get; set; }
        public string? TransactionRef { get; set; }

        public Car? Car { get; set; }
        public PaymentGateway? PaymentGateway { get; set; }
    }
}
