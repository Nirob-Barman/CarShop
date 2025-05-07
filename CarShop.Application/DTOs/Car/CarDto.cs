
namespace CarShop.Application.DTOs.Car
{
    public class CarDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
        public int BrandId { get; set; }
        public string? BrandName { get; set; }
    }
}
