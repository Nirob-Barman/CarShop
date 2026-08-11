namespace CarShop.Domain.Entities
{
    public class StockAlert : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public int CarId { get; set; }
        public bool IsTriggered { get; private set; }
        public DateTime SubscribedAt { get; set; }
        public DateTime? TriggeredAt { get; private set; }
        public Car? Car { get; set; }

        public void Trigger()
        {
            IsTriggered = true;
            TriggeredAt = DateTime.UtcNow;
        }
    }
}
