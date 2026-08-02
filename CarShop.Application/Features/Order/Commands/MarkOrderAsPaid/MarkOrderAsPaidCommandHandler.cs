using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using System.Text.Json;
using OrderEntity = CarShop.Domain.Entities.Order;

namespace CarShop.Application.Features.Order.Commands.MarkOrderAsPaid
{
    public class MarkOrderAsPaidCommandHandler : IRequestHandler<MarkOrderAsPaidCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _auditLogService;

        public MarkOrderAsPaidCommandHandler(IUnitOfWork unitOfWork, IAuditLogService auditLogService)
        {
            _unitOfWork = unitOfWork;
            _auditLogService = auditLogService;
        }

        public async Task<Result<string>> Handle(MarkOrderAsPaidCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Repository<OrderEntity>().GetByIdAsync(request.OrderId);
            if (order == null) return Result<string>.Fail("Order not found.");
            if (order.Status == "Confirmed") return Result<string>.Ok(null, "Already confirmed.");

            var oldStatus = order.Status;
            order.Status = "Confirmed";
            _unitOfWork.Repository<OrderEntity>().Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync("Order", "Confirmed",
                order.UserId, null,
                $"Payment confirmed for order #{request.OrderId}",
                entityId: request.OrderId,
                oldValues: JsonSerializer.Serialize(new { Status = oldStatus }),
                newValues: JsonSerializer.Serialize(new { Status = "Confirmed" }));

            return Result<string>.Ok(null, "Order confirmed.");
        }
    }
}
