using CarShop.Application.Interfaces;
using CarShop.Application.DTOs.Order;
using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using CarShop.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Order.Queries.GetAllOrders
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, Result<PagedResult<OrderDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;

        public GetAllOrdersQueryHandler(IApplicationDbContext context, IIdentityService identityService)
        {
            _context = context;
            _identityService = identityService;
        }

        public async Task<Result<PagedResult<OrderDto>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            OrderStatus? statusFilter = Enum.TryParse<OrderStatus>(request.Status, ignoreCase: true, out var parsed) ? parsed : null;

            var allOrders = await _context.Orders.Include(o => o.Car)
                .Where(o => statusFilter == null || o.Status == statusFilter)
                .ToListAsync(cancellationToken);

            var ordered = allOrders.OrderByDescending(o => o.OrderedAt).ToList();
            var totalCount = ordered.Count;

            var pagedItems = ordered.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();

            var dtos = new List<OrderDto>();
            foreach (var o in pagedItems)
            {
                var user = await _identityService.FindByIdAsync(o.UserId ?? "");
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
                    Status = o.Status.ToString(),
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
