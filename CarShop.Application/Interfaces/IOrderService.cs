using CarShop.Application.DTOs.Order;
using CarShop.Application.Wrappers;

namespace CarShop.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Result<string>> PlaceOrderAsync(int carId, string? promoCode = null);
        Task<Result<(int OrderId, decimal FinalPrice, string CarTitle)>> CreatePendingOrderAsync(int carId, string? promoCode = null);
        Task SetOrderGatewayAsync(int orderId, int paymentGatewayId);
        Task<Result<string>> MarkOrderAsPaidAsync(int orderId);
        Task<Result<string>> CancelPendingOrderByIdAsync(int orderId);
        Task ExpireStalePendingOrdersAsync(int olderThanMinutes = 30);
        Task<Result<IEnumerable<OrderDto>>> GetOrdersByUserIdAsync();
        Task<Result<OrderDto>> GetOrderByIdAsync(int orderId);
        Task<Result<OrderDto>> GetOrderByIdAdminAsync(int orderId);
        Task<Result<string>> CancelOrderAsync(int orderId);
        Task<Result<PagedResult<OrderDto>>> GetAllOrdersAsync(string? status = null, int page = 1, int pageSize = 20);
        Task<Result<string>> AdminCancelOrderAsync(int orderId);
        Task<int> GetCompletedOrdersCountAsync();
    }
}
