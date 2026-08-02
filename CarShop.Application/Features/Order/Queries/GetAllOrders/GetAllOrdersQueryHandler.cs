using CarShop.Application.DTOs.Order;
using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using OrderEntity = CarShop.Domain.Entities.Order;

namespace CarShop.Application.Features.Order.Queries.GetAllOrders
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, Result<PagedResult<OrderDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserManager _userManager;

        public GetAllOrdersQueryHandler(IUnitOfWork unitOfWork, IUserManager userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<Result<PagedResult<OrderDto>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var allOrders = await _unitOfWork.Repository<OrderEntity>().GetAllWithIncludesAsync(
                predicate: o => request.Status == null || o.Status == request.Status,
                selector: o => o,
                o => o.Car!
            );

            var ordered = allOrders.OrderByDescending(o => o.OrderedAt).ToList();
            var totalCount = ordered.Count;

            var pagedItems = ordered.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();

            var dtos = new List<OrderDto>();
            foreach (var o in pagedItems)
            {
                var user = await _userManager.FindByIdAsync(o.UserId ?? "");
                dtos.Add(new OrderDto
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
                    FinalPrice = o.FinalPrice > 0 ? o.FinalPrice : o.Car?.Price ?? 0,
                    UserEmail = user?.Email,
                    UserFullName = user?.FullName
                });
            }

            return Result<PagedResult<OrderDto>>.Ok(new PagedResult<OrderDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            });
        }
    }
}
