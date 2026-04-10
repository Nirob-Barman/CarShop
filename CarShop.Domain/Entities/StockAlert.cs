namespace CarShop.Domain.Entities
{
    public class StockAlert
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int CarId { get; set; }
        public bool IsTriggered { get; set; }
        public DateTime SubscribedAt { get; set; }
        public DateTime? TriggeredAt { get; set; }
        public Car? Car { get; set; }
    }
}
