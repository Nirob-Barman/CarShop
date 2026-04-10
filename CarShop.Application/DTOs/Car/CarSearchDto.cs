namespace CarShop.Application.DTOs.Car
{
    public class CarSearchDto
    {
        public string? Keyword { get; set; }
        public string? BrandName { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string SortBy { get; set; } = "newest";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
