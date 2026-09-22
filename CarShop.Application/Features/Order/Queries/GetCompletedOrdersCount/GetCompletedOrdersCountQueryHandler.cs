using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using CarShop.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Order.Queries.GetCompletedOrdersCount
{
    public class GetCompletedOrdersCountQueryHandler : IRequestHandler<GetCompletedOrdersCountQuery, Result<int>>
    {
        private readonly IApplicationDbContext _context;

        public GetCompletedOrdersCountQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> Handle(GetCompletedOrdersCountQuery request, CancellationToken cancellationToken)
        {
            var count = await _context.Orders
                .CountAsync(o => o.Status == OrderStatus.Confirmed, cancellationToken);
            return Result<int>.Ok(count);
        }
    }
}
