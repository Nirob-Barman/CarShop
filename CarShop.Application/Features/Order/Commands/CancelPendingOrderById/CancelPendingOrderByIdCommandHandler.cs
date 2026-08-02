using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using System.Text.Json;
using CarEntity = CarShop.Domain.Entities.Car;
using OrderEntity = CarShop.Domain.Entities.Order;

namespace CarShop.Application.Features.Order.Commands.CancelPendingOrderById
{
    public class CancelPendingOrderByIdCommandHandler : IRequestHandler<CancelPendingOrderByIdCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _auditLogService;

        public CancelPendingOrderByIdCommandHandler(IUnitOfWork unitOfWork, IAuditLogService auditLogService)
        {
            _unitOfWork = unitOfWork;
            _auditLogService = auditLogService;
        }

        public async Task<Result<string>> Handle(CancelPendingOrderByIdCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Repository<OrderEntity>().GetByIdAsync(request.OrderId);

            if (order == null || order.Status != "Pending")
                return Result<string>.Ok(null, "Nothing to cancel.");

            var car = await _unitOfWork.Repository<CarEntity>().GetByIdAsync(order.CarId);
            if (car != null) { car.Quantity += order.Quantity; _unitOfWork.Repository<CarEntity>().Update(car); }

            var oldStatus = order.Status;
            order.Status = "Cancelled";
            _unitOfWork.Repository<OrderEntity>().Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync("Order", "Cancel",
                order.UserId, null,
                $"Pending order #{order.Id} cancelled",
                entityId: order.Id,
                oldValues: JsonSerializer.Serialize(new { Status = oldStatus }),
                newValues: JsonSerializer.Serialize(new { Status = "Cancelled" }));

            return Result<string>.Ok(null, "Order cancelled.");
        }
    }
}
