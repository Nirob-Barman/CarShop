using CarShop.Application.DTOs.Order;
using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Order.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetOrderByIdQueryHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var order = await _context.Orders.AsNoTracking()
                .Where(o => o.Id == request.OrderId && o.UserId == userId)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    UserId = o.UserId!,
                    CarId = o.CarId,
                    OrderedAt = o.OrderedAt,
                    Quantity = o.Quantity,
                    CarTitle = o.Car != null ? o.Car.Title : "N/A",
                    CarPrice = o.Car != null ? o.Car.Price : 0,
                    CarImageUrl = o.Car != null ? o.Car.ImageUrl : null,
                    Status = o.Status.ToString(),
                    PromoCode = o.PromoCode,
                    DiscountAmount = o.DiscountAmount,
                    FinalPrice = o.FinalPrice > 0 ? o.FinalPrice : o.Car != null ? o.Car.Price : 0
                }).FirstOrDefaultAsync(cancellationToken);

            if (order == null)
                return Result<OrderDto>.Fail("Order not found.");

            return Result<OrderDto>.Ok(order);
        }
    }
}
