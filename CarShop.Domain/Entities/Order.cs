namespace CarShop.Domain.Entities
{
    public class Order : BaseEntity
    {
        public string? UserId { get; set; }
        public int CarId { get; set; }
        public DateTime OrderedAt { get; set; }
        public int Quantity { get; set; }
        public OrderStatus Status { get; private set; } = OrderStatus.Pending;
        public string? PromoCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
        public int? PaymentGatewayId { get; set; }
        public string? TransactionRef { get; set; }

        public Car? Car { get; set; }
        public PaymentGateway? PaymentGateway { get; set; }

        public void Confirm()
        {
            if (Status == OrderStatus.Confirmed) return;
            Status = OrderStatus.Confirmed;
        }

        public bool Cancel()
        {
            if (Status == OrderStatus.Cancelled) return false;
            Status = OrderStatus.Cancelled;
            return true;
        }
    }
}
