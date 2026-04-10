using CarShop.Application.DTOs.Analytics;
using CarShop.Application.Wrappers;

namespace CarShop.Application.Interfaces
{
    public interface IAnalyticsService
    {
        Task<Result<AnalyticsDashboardDto>> GetDashboardAsync();
    }
}
