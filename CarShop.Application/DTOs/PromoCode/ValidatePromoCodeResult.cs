namespace CarShop.Application.DTOs.PromoCode
{
    public class ValidatePromoCodeResult
    {
        public bool IsValid { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public string? Message { get; set; }
        public int PromoCodeId { get; set; }
    }
}
