namespace CarShop.Web.ViewModels.Order
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string? CarTitle { get; set; }
        public decimal CarPrice { get; set; }
        public string? CarImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderedAt { get; set; }
    }
}
