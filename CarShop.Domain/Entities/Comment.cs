namespace CarShop.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public string? UserName { get; set; }
        public string? Content { get; private set; }
        public DateTime CreatedAt { get; set; }

        public int CarId { get; set; }
        public int? Rating { get; private set; }
        public string? UserId { get; set; }
        public Car? Car { get; set; }

        public Comment(string content, int? rating)
        {
            Edit(content, rating);
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
