using CarShop.Application.DTOs.Order;
using CarShop.Application.Interfaces;
using CarShop.Domain.Entities;
using CarShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task PlaceOrderAsync(string userId, int carId)
        {
            // Check if the car exists and has enough quantity
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == carId);
            if (car == null || car.Quantity <= 0)
                throw new Exception("Car not available or out of stock.");

            // Reduce car quantity
            car.Quantity--;

            // Create the order
            var order = new Order
            {
                UserId = userId,
                CarId = carId,
                OrderedAt = DateTime.UtcNow,
                Quantity = 1
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.Car)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderedAt)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    UserId = o.UserId!,
                    CarId = o.CarId,
                    OrderedAt = o.OrderedAt,
                    Quantity = o.Quantity,
                    CarTitle = o.Car!.Title,
                    CarPrice = o.Car.Price,
                    CarImageUrl = o.Car.ImageUrl
                }).ToListAsync();
        }
    }
}
