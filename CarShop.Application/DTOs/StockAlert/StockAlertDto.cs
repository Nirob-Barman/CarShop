namespace CarShop.Application.DTOs.StockAlert
{
    public class StockAlertDto
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string? CarTitle { get; set; }
        public string? CarImageUrl { get; set; }
        public bool IsTriggered { get; set; }
        public DateTime SubscribedAt { get; set; }
        public DateTime? TriggeredAt { get; set; }
    }
}
