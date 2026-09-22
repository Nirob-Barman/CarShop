using CarShop.Application.DTOs.Order;
using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Order.Queries.GetOrdersByUserId
{
    public class GetOrdersByUserIdQueryHandler : IRequestHandler<GetOrdersByUserIdQuery, Result<IEnumerable<OrderDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetOrdersByUserIdQueryHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<IEnumerable<OrderDto>>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var orders = await _context.Orders.AsNoTracking()
                .Where(o => o.UserId == userId).OrderByDescending(o => o.OrderedAt)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    UserId = o.UserId!,
                    CarId = o.CarId,
                    OrderedAt = o.OrderedAt,
                    Quantity = o.Quantity,
                    CarTitle = o.Car != null ? o.Car.Title :  "N/A",
                    CarPrice = o.Car != null ? o.Car.Price : 0,
                    CarImageUrl = o.Car != null ? o.Car.ImageUrl : null,
                    Status = o.Status.ToString(),
                    PromoCode = o.PromoCode,
                    DiscountAmount = o.DiscountAmount,
                    FinalPrice = o.FinalPrice > 0 ? o.FinalPrice : o.Car != null ? o.Car.Price : 0
                })
                .ToListAsync(cancellationToken);

            return Result<IEnumerable<OrderDto>>.Ok(orders);
        }
    }
}
