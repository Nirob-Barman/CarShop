using CarShop.Application.DTOs.Order;
using CarShop.Application.Wrappers;

namespace CarShop.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Result<string>> PlaceOrderAsync(string userId, int carId);
        Task<Result<IEnumerable<OrderDto>>> GetOrdersByUserIdAsync(string userId);
        Task<Result<string>> CancelOrderAsync(int orderId, string userId);
    }
}
