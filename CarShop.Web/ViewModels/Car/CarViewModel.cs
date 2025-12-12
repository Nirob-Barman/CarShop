using System.ComponentModel.DataAnnotations;

namespace CarShop.Web.ViewModels.Car
{
    public class CarViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Car title is required.")]
        [StringLength(100, ErrorMessage = "Car title cannot be longer than 100 characters.")]
        public string? Title { get; set; }
        //[Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string? Description { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal Price { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid brand.")]
        public int BrandId { get; set; }
        [StringLength(100, ErrorMessage = "Brand name cannot be longer than 100 characters.")]
        public string? BrandName { get; set; }
    }
}
