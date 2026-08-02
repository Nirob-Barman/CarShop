using CarShop.Application.DTOs.Order;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using OrderEntity = CarShop.Domain.Entities.Order;

namespace CarShop.Application.Features.Order.Queries.GetOrdersByUserId
{
    public class GetOrdersByUserIdQueryHandler : IRequestHandler<GetOrdersByUserIdQuery, Result<IEnumerable<OrderDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public GetOrdersByUserIdQueryHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<IEnumerable<OrderDto>>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var orders = await _unitOfWork.Repository<OrderEntity>().GetAllWithIncludesAsync(
                predicate: o => o.UserId == userId,
                selector: o => o,
                o => o.Car!
            );

            var dtos = orders.OrderByDescending(o => o.OrderedAt).Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId!,
                CarId = o.CarId,
                OrderedAt = o.OrderedAt,
                Quantity = o.Quantity,
                CarTitle = o.Car?.Title ?? "N/A",
                CarPrice = o.Car?.Price ?? 0,
                CarImageUrl = o.Car?.ImageUrl,
                Status = o.Status,
                PromoCode = o.PromoCode,
                DiscountAmount = o.DiscountAmount,
                FinalPrice = o.FinalPrice > 0 ? o.FinalPrice : o.Car?.Price ?? 0
            });

            return Result<IEnumerable<OrderDto>>.Ok(dtos);
        }
    }
}
