using CarShop.Application.DTOs.Order;
using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using OrderEntity = CarShop.Domain.Entities.Order;

namespace CarShop.Application.Features.Order.Queries.GetOrderByIdAdmin
{
    public class GetOrderByIdAdminQueryHandler : IRequestHandler<GetOrderByIdAdminQuery, Result<OrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserManager _userManager;

        public GetOrderByIdAdminQueryHandler(IUnitOfWork unitOfWork, IUserManager userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<Result<OrderDto>> Handle(GetOrderByIdAdminQuery request, CancellationToken cancellationToken)
        {
            var order = (await _unitOfWork.Repository<OrderEntity>().GetAllWithIncludesAsync(
                o => o.Id == request.OrderId,
                o => o,
                o => o.Car!)).FirstOrDefault();

            if (order == null)
                return Result<OrderDto>.Fail("Order not found.");

            var user = await _userManager.FindByIdAsync(order.UserId ?? "");

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
