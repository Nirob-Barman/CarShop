using CarShop.Application.DTOs.Order;

namespace CarShop.Application.Interfaces
{
    public interface IOrderService
    {
        Task PlaceOrderAsync(string userId, int carId);
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId);
        Task CancelOrderAsync(int orderId, string userId);
    }
}
