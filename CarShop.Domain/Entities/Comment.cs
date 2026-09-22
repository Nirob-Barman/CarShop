namespace CarShop.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public string? UserName { get; private set; }
        public string? Content { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public int CarId { get; private set; }
        public int? Rating { get; private set; }
        public string? UserId { get; private set; }
        public Car? Car { get; private set; }

        private Comment(
            string content,
            int? rating,
            int carId,
            string userId,
            string userName)
        {
            Edit(content, rating);

            if (carId <= 0)
                throw new ArgumentOutOfRangeException(nameof(carId));

            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required.", nameof(userId));

            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("User name is required.", nameof(userName));

            CarId = carId;
            UserId = userId;
            UserName = userName.Trim();
            CreatedAt = DateTime.UtcNow;
        }

        public static Comment Create(string content,
            int? rating,
            int carId,
            string userId,
            string userName)
        {
            return new Comment(
                content,
                rating,
                carId,
                userId,
                userName);
        }

        public void Edit(string content, int? rating)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Review content is required.", nameof(content));
            if (rating.HasValue && (rating < 1 || rating > 5))
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");

            Content = content.Trim();
            Rating = rating;
        }
    }
}
