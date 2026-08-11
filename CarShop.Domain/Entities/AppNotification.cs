namespace CarShop.Domain.Entities
{
    public class AppNotification : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Link { get; set; }
        public bool IsRead { get; private set; }
        public DateTime CreatedAt { get; set; }

        public void MarkAsRead() => IsRead = true;
    }
}
