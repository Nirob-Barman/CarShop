namespace CarShop.Application.DTOs.Analytics
{
    public class AnalyticsDashboardDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalCars { get; set; }
        public int TotalUsers { get; set; }
        public List<TopCarDto> TopFiveCars { get; set; } = new();
        public List<LowStockCarDto> LowStockCars { get; set; } = new();
        public List<OrdersPerMonthDto> OrdersPerMonth { get; set; } = new();
    }

    public class TopCarDto
    {
        public string CarTitle { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
    }

    public class LowStockCarDto
    {
        public int CarId { get; set; }
        public string CarTitle { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

    public class OrdersPerMonthDto
    {
        public string Month { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
