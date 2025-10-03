
using CarShop.Application.DTOs.Order;
using CarShop.Application.Interfaces.Repositories;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;

namespace CarShop.Application.Services
{
    internal class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICarRepository _carRepository;

        public OrderService(IOrderRepository orderRepository, ICarRepository carRepository)
        {
            _orderRepository = orderRepository;
            _carRepository = carRepository;
        }

        public async Task<Result<string>> PlaceOrderAsync(string userId, int carId)
        {
            var car = await _carRepository.GetByIdAsync(carId);

            if (car == null)
                return Result<string>.Fail("Car not found.");

            if (car.Quantity <= 0)
                return Result<string>.Fail("Car is out of stock.");

            // Reduce stock
            car.Quantity--;

            var order = new Order
            {
                UserId = userId,
                CarId = carId,
                OrderedAt = DateTime.UtcNow,
                Quantity = 1
            };

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            return Result<string>.Ok(null, "Order placed successfully.");
        }

        public async Task<Result<IEnumerable<OrderDto>>> GetOrdersByUserIdAsync(string userId)
        {
            var orders = await _orderRepository.GetByUserIdAsync(userId);

            var dtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId!,
                CarId = o.CarId,
                OrderedAt = o.OrderedAt,
                Quantity = o.Quantity,
                CarTitle = o.Car?.Title ?? "N/A",
                CarPrice = o.Car?.Price ?? 0,
                CarImageUrl = o.Car?.ImageUrl
            });

            return Result<IEnumerable<OrderDto>>.Ok(dtos);
        }

        public async Task<Result<string>> CancelOrderAsync(int orderId, string userId)
        {
            var order = await _orderRepository.GetByIdAndUserIdAsync(orderId, userId);

            if (order == null)
                return Result<string>.Fail("Order not found or you are not authorized to cancel it.");

            // Restore stock
            if (order.Car != null)
            {
                order.Car.Quantity += order.Quantity;
            }

            _orderRepository.DeleteAsync(order);
            await _orderRepository.SaveChangesAsync();

            return Result<string>.Ok(null, "Order cancelled successfully.");
        }
    }
}
