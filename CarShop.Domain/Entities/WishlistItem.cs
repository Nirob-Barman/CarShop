namespace CarShop.Domain.Entities
{
    public class WishlistItem : BaseEntity
    {
        public string UserId { get; private set; } = string.Empty;
        public int CarId { get; private set; }
        public DateTime AddedAt { get; private set; }
        public Car? Car { get; private set; }

        private WishlistItem()
        {
        }

        private WishlistItem(string userId, int carId)
        {
            UserId = userId;
            CarId = carId;
            AddedAt = DateTime.UtcNow;
        }

        public static WishlistItem Create(string userId, int carId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required.", nameof(userId));

            if (carId <= 0)
                throw new ArgumentException("Car ID must be greater than zero.", nameof(carId));

            return new WishlistItem(userId, carId);
        }

    }
}
