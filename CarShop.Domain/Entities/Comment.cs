
namespace CarShop.Domain.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public int CarId { get; set; }
        public Car? Car { get; set; }
    }
}
