using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using System.Text.Json;
using CarShop.Domain.Enums;

namespace CarShop.Application.Features.Order.Commands.MarkOrderAsPaid
{
    public class MarkOrderAsPaidCommandHandler : IRequestHandler<MarkOrderAsPaidCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;

        public MarkOrderAsPaidCommandHandler(IApplicationDbContext context, IAuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        public async Task<Result<string>> Handle(MarkOrderAsPaidCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FindAsync(request.OrderId);
            if (order == null) return Result<string>.Fail("Order not found.");
            if (order.Status == OrderStatus.Confirmed) return Result<string>.Ok(null, "Already confirmed.");

            var oldStatus = order.Status;
            order.Confirm();
            _context.Orders.Update(order);
            await _context.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync("Order", "Confirmed",
                order.UserId, null,
                $"Payment confirmed for order #{request.OrderId}",
                entityId: request.OrderId,
                oldValues: JsonSerializer.Serialize(new { Status = oldStatus.ToString() }),
                newValues: JsonSerializer.Serialize(new { Status = "Confirmed" }));

            return Result<string>.Ok(null, "Order confirmed.");
        }
    }
}
