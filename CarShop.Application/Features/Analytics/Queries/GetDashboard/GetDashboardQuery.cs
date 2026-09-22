using CarShop.Application.DTOs.Analytics;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Analytics.Queries.GetDashboard
{
    public record GetDashboardQuery : IRequest<Result<AnalyticsDashboardDto>>;
}
