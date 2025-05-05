
namespace CarShop.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int CarId { get; set; }
        public DateTime OrderedAt { get; set; }
        public int Quantity { get; set; }

        public Car? Car { get; set; }
    }
}
