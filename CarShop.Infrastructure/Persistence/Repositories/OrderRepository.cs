

using CarShop.Application.Interfaces.Repositories;
using CarShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task<List<Order>> GetByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.Car)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAndUserIdAsync(int orderId, string userId)
        {
            return await _context.Orders
                .Include(o => o.Car)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
        }

        public void DeleteAsync(Order order)
        {
            _context.Orders.Remove(order);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
