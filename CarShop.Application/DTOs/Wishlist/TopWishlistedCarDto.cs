namespace CarShop.Application.DTOs.Wishlist
{
    public class TopWishlistedCarDto
    {
        public int CarId { get; set; }
        public string? CarTitle { get; set; }
        public decimal CarPrice { get; set; }
        public string? CarImageUrl { get; set; }
        public string? BrandName { get; set; }
        public int WishlistCount { get; set; }
    }
}
