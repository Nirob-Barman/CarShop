
namespace CarShop.Application.DTOs.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int CarId { get; set; }
        public DateTime OrderedAt { get; set; }
        public int Quantity { get; set; }

        public string? CarTitle { get; set; }
        public decimal CarPrice { get; set; }
        public string? CarImageUrl { get; set; }
        public decimal TotalPrice => CarPrice * Quantity;

        public string Status { get; set; } = "Confirmed";
        public string? PromoCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
        public string? UserEmail { get; set; }
        public string? UserFullName { get; set; }
    }
}
