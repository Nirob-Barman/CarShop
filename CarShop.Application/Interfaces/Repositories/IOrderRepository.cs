
using CarShop.Domain.Entities;

namespace CarShop.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<List<Order>> GetByUserIdAsync(string userId);
        Task<Order?> GetByIdAndUserIdAsync(int orderId, string userId);
        void DeleteAsync(Order order);
        Task SaveChangesAsync();
    }
}
