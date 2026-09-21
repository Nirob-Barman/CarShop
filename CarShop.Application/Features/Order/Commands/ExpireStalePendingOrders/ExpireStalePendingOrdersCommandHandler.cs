using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using CarShop.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Order.Commands.ExpireStalePendingOrders
{
    public class ExpireStalePendingOrdersCommandHandler : IRequestHandler<ExpireStalePendingOrdersCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public ExpireStalePendingOrdersCommandHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(ExpireStalePendingOrdersCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var cutoff = DateTime.UtcNow.AddMinutes(-request.OlderThanMinutes);
            var stale = await _context.Orders.Include(o => o.Car)
                .Where(o => o.UserId == userId && o.Status == OrderStatus.Pending && o.OrderedAt < cutoff)
                .ToListAsync(cancellationToken);

            foreach (var order in stale)
            {
                if (order.Car != null)
                {
                    order.Car.RestoreStock(order.Quantity);
                    _context.Cars.Update(order.Car);
                }
                order.Cancel();
                _context.Orders.Update(order);
            }

            if (stale.Any())
                await _context.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "Stale pending orders expired.");
        }
    }
}
