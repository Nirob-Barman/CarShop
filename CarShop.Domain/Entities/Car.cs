
namespace CarShop.Domain.Entities
{
    public class Car : BaseEntity
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }

        public int BrandId { get; set; }
        public Brand? Brand { get; set; }

        public ICollection<Comment>? Comments { get; set; }
        //public ICollection<Order>? Orders { get; set; }

        public bool DecrementStock(int quantity)
        {
            if (quantity <= 0 || Quantity < quantity) return false;
            Quantity -= quantity;
            return true;
        }

        public void RestoreStock(int quantity)
        {
            if (quantity <= 0) return;
            Quantity += quantity;
        }
    }
}
