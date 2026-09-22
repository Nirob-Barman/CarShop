namespace CarShop.Domain.Entities
{
    public class StockAlert : BaseEntity
    {
        public string UserId { get; private set; } = string.Empty;
        public int CarId { get; private set; }
        public bool IsTriggered { get; private set; }
        public DateTime SubscribedAt { get; private set; }
        public DateTime? TriggeredAt { get; private set; }
        public Car? Car { get; private set; }

        private StockAlert(
            string userId,
            int carId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required.", nameof(userId));

            if (carId <= 0)
                throw new ArgumentOutOfRangeException(nameof(carId), "Car ID must be greater than zero.");

            UserId = userId;
            CarId = carId;

            SubscribedAt = DateTime.UtcNow;
            IsTriggered = false;
        }

        public static StockAlert Create(
            string userId,
            int carId)
        {
            return new StockAlert(userId, carId);
        }

        public void Trigger()
        {
            IsTriggered = true;
            TriggeredAt = DateTime.UtcNow;
        }
    }
}
