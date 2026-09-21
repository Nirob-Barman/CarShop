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
            var order = await _context.Orders.Include(o => o.Car)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

            if (order == null)
                return Result<OrderDto>.Fail("Order not found.");

            var user = await _identityService.FindByIdAsync(order.UserId ?? "");

            return Result<OrderDto>.Ok(new OrderDto
            {
                Id             = order.Id,
                UserId         = order.UserId!,
                CarId          = order.CarId,
                OrderedAt      = order.OrderedAt,
                Quantity       = order.Quantity,
                CarTitle       = order.Car?.Title ?? "N/A",
                CarPrice       = order.Car?.Price ?? 0,
                CarImageUrl    = order.Car?.ImageUrl,
                Status         = order.Status.ToString(),
                PromoCode      = order.PromoCode,
                DiscountAmount = order.DiscountAmount,
                FinalPrice     = order.FinalPrice > 0 ? order.FinalPrice : order.Car?.Price ?? 0,
                UserEmail      = user?.Email,
                UserFullName   = user?.FullName
            });
        }
    }
}
