
namespace CarShop.Domain.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public int CarId { get; set; }
        public int? Rating { get; set; }
        public string? UserId { get; set; }
        public Car? Car { get; set; }
    }
}
