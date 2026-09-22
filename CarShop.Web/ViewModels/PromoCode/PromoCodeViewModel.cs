namespace CarShop.Web.ViewModels.PromoCode
{
    public class PromoCodeViewModel
    {
        public string Code { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public int? MaxUsages { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
