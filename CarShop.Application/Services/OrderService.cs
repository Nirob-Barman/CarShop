using CarShop.Application.DTOs.Order;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;

namespace CarShop.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> PlaceOrderAsync(string userId, int carId)
        {
            var car = await _unitOfWork.Repository<Car>().GetByIdAsync(carId);

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

            await _unitOfWork.Repository<Order>().AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Ok(null, "Order placed successfully.");
        }

        public async Task<Result<IEnumerable<OrderDto>>> GetOrdersByUserIdAsync(string userId)
        {
            var orders = await _unitOfWork.Repository<Order>().GetAllWithIncludesAsync(
                predicate: o => o.UserId == userId,
                selector: o => o,
                o => o.Car!
            );

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
            var order = (await _unitOfWork.Repository<Order>().GetAllWithIncludesAsync(
                o => o.Id == orderId && o.UserId == userId,
                o => o,
                o => o.Car!)).FirstOrDefault();

            if (order == null)
                return Result<string>.Fail("Order not found or you are not authorized to cancel it.");

            // Restore stock
            if (order.Car != null)
            {
                order.Car.Quantity += order.Quantity;
            }

            _unitOfWork.Repository<Order>().Remove(order);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Ok(null, "Order cancelled successfully.");
        }
    }
}
