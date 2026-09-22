namespace CarShop.Domain.Entities
{
    public class AppNotification : BaseEntity
    {
        public string UserId { get; private set; } = string.Empty;
        public string Message { get; private set; } = string.Empty;
        public string? Link { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private AppNotification(
            string userId,
            string message,
            string? link)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException(
                    "User ID is required.",
                    nameof(userId));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException(
                    "Notification message is required.",
                    nameof(message));

            UserId = userId;
            Message = message.Trim();
            Link = string.IsNullOrWhiteSpace(link) ? null : link.Trim();
            CreatedAt = DateTime.UtcNow;
            IsRead = false;
        }

        public static AppNotification Create(
            string userId,
            string message,
            string? link = null)
        {
            return new AppNotification(userId, message, link);
        }

        public void MarkAsRead() => IsRead = true;
    }
}
