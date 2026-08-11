using CarShop.Application.Features.Notification.Commands.CreateNotification;
using CarShop.Application.Features.StockAlert.Commands.TriggerStockAlerts;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using System.Text.Json;
using CarShop.Domain.Entities;
using CarShop.Domain.Enums;
using OrderEntity = CarShop.Domain.Entities.Order;

namespace CarShop.Application.Features.Order.Commands.CancelOrder
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public CancelOrderCommandHandler(
            IUnitOfWork unitOfWork,
            IMediator mediator,
            IAuditLogService auditLogService,
            IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var order = (await _unitOfWork.Repository<OrderEntity>().GetAllWithIncludesAsync(
                o => o.Id == request.OrderId && o.UserId == userId,
                o => o,
                o => o.Car!)).FirstOrDefault();

            if (order == null)
                return Result<string>.Fail("Order not found or you are not authorized to cancel it.");

            if (order.Status == OrderStatus.Cancelled)
                return Result<string>.Fail("Order is already cancelled.");

            bool wasOutOfStock = order.Car != null && order.Car.Quantity == 0;

            var oldStatus = order.Status;

            if (order.Car != null)
                order.Car.RestoreStock(order.Quantity);

            order.Cancel();
            _unitOfWork.Repository<OrderEntity>().Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync("Order", "UserCancel",
                userId, null,
                $"User cancelled order #{request.OrderId} for car '{order.Car?.Title}'",
                entityId: request.OrderId,
                oldValues: JsonSerializer.Serialize(new { Status = oldStatus.ToString() }),
                newValues: JsonSerializer.Serialize(new { Status = "Cancelled" }));

            if (wasOutOfStock && order.Car != null)
                await _mediator.Send(new TriggerStockAlertsCommand(order.Car.Id));

            await _mediator.Send(new CreateNotificationCommand(userId,
                $"Your order for {order.Car?.Title ?? "a car"} has been cancelled.",
                "/Order/MyOrders"));

            return Result<string>.Ok(null, "Order cancelled successfully.");
        }
    }
}
