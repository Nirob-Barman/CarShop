using CarShop.Application.Interfaces;
using CarShop.Application.DTOs.Order;
using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Order.Queries.GetOrderByIdAdmin
{
    public class GetOrderByIdAdminQueryHandler : IRequestHandler<GetOrderByIdAdminQuery, Result<OrderDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;

        public GetOrderByIdAdminQueryHandler(IApplicationDbContext context, IIdentityService identityService)
        {
            _context = context;
            _identityService = identityService;
        }

        public async Task<Result<OrderDto>> Handle(GetOrderByIdAdminQuery request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.AsNoTracking()
                .Where(o => o.Id == request.OrderId)
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

            var user = await _identityService.FindByIdAsync(order.UserId ?? "");
            order.UserEmail = user?.Email;
            order.UserFullName = user?.FullName;

            return Result<OrderDto>.Ok(order);
        }
    }
}
