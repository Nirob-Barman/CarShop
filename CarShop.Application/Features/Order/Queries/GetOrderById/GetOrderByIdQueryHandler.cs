using CarShop.Application.DTOs.Order;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using OrderEntity = CarShop.Domain.Entities.Order;

namespace CarShop.Application.Features.Order.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public GetOrderByIdQueryHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var order  = (await _unitOfWork.Repository<OrderEntity>().GetAllWithIncludesAsync(
                o => o.Id == request.OrderId && o.UserId == userId,
                o => o,
                o => o.Car!)).FirstOrDefault();

            if (order == null)
                return Result<OrderDto>.Fail("Order not found.");

            var dto = new OrderDto
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
                FinalPrice     = order.FinalPrice > 0 ? order.FinalPrice : order.Car?.Price ?? 0
            };

            return Result<OrderDto>.Ok(dto);
        }
    }
}
