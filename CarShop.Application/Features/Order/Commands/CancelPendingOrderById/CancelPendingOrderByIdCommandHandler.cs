using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using System.Text.Json;
using CarShop.Domain.Enums;

namespace CarShop.Application.Features.Order.Commands.CancelPendingOrderById
{
    public class CancelPendingOrderByIdCommandHandler : IRequestHandler<CancelPendingOrderByIdCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;

        public CancelPendingOrderByIdCommandHandler(IApplicationDbContext context, IAuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        public async Task<Result<string>> Handle(CancelPendingOrderByIdCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FindAsync(request.OrderId);

            if (order == null || order.Status != OrderStatus.Pending)
                return Result<string>.Ok(null, "Nothing to cancel.");

            var car = await _context.Cars.FindAsync(order.CarId);
            if (car != null) { car.RestoreStock(order.Quantity); _context.Cars.Update(car); }

            var oldStatus = order.Status;
            order.Cancel();
            _context.Orders.Update(order);
            await _context.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync("Order", "Cancel",
                order.UserId, null,
                $"Pending order #{order.Id} cancelled",
                entityId: order.Id,
                oldValues: JsonSerializer.Serialize(new { Status = oldStatus.ToString() }),
                newValues: JsonSerializer.Serialize(new { Status = "Cancelled" }));

            return Result<string>.Ok(null, "Order cancelled.");
        }
    }
}
