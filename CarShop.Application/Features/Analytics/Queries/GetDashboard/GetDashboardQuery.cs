using CarShop.Application.DTOs.Analytics;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Analytics.Queries.GetDashboard
{
    public class GetDashboardQuery : IRequest<Result<AnalyticsDashboardDto>>
    {
    }
}
