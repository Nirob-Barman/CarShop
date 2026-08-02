using CarShop.Application.DTOs.Analytics;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using CarEntity = CarShop.Domain.Entities.Car;
using OrderEntity = CarShop.Domain.Entities.Order;

namespace CarShop.Application.Features.Analytics.Queries.GetDashboard
{
    public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, Result<AnalyticsDashboardDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDashboardQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AnalyticsDashboardDto>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
        {
            var orders = await _unitOfWork.Repository<OrderEntity>().GetAllWithIncludesAsync(
                predicate: o => o.Status != "Cancelled",
                selector: o => o,
                o => o.Car!
            );

            var orderList = orders.ToList();

            var totalOrders = orderList.Count;
            var totalRevenue = orderList.Sum(o => o.FinalPrice > 0 ? o.FinalPrice : o.Car?.Price ?? 0);

            var topCars = orderList
                .GroupBy(o => new { o.CarId, Title = o.Car?.Title ?? "Unknown" })
                .Select(g => new TopCarDto
                {
                    CarTitle = g.Key.Title,
                    OrderCount = g.Count(),
                    Revenue = g.Sum(o => o.FinalPrice > 0 ? o.FinalPrice : o.Car?.Price ?? 0)
                })
                .OrderByDescending(t => t.OrderCount)
                .Take(5)
                .ToList();

            var allCars = await _unitOfWork.Repository<CarEntity>().GetAllAsync();
            var carList = allCars.ToList();

            var lowStockCars = carList
                .Where(c => c.Quantity < 5)
                .OrderBy(c => c.Quantity)
                .Select(c => new LowStockCarDto
                {
                    CarId = c.Id,
                    CarTitle = c.Title ?? string.Empty,
                    Quantity = c.Quantity
                })
                .ToList();

            var ordersPerMonth = orderList
                .GroupBy(o => new { o.OrderedAt.Year, o.OrderedAt.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new OrdersPerMonthDto
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Count = g.Count()
                })
                .ToList();

            var totalCars = carList.Count;

            var dashboard = new AnalyticsDashboardDto
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                TotalCars = totalCars,
                TopFiveCars = topCars,
                LowStockCars = lowStockCars,
                OrdersPerMonth = ordersPerMonth
            };

            return Result<AnalyticsDashboardDto>.Ok(dashboard);
        }
    }
}
