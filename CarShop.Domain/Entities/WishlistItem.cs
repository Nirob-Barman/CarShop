namespace CarShop.Domain.Entities
{
    public class WishlistItem
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int CarId { get; set; }
        public DateTime AddedAt { get; set; }
        public Car? Car { get; set; }
    }
}
