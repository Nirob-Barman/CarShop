using CarShop.Application.Features.Notification.Commands.CreateNotification;
using CarShop.Application.Features.StockAlert.Commands.TriggerStockAlerts;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using System.Text.Json;
using CarShop.Domain.Entities;
using OrderEntity = CarShop.Domain.Entities.Order;

namespace CarShop.Application.Features.Order.Commands.AdminCancelOrder
{
    public class AdminCancelOrderCommandHandler : IRequestHandler<AdminCancelOrderCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IEmailService _emailService;
        private readonly IUserManager _userManager;
        private readonly IAuditLogService _auditLogService;

        public AdminCancelOrderCommandHandler(
            IUnitOfWork unitOfWork,
            IMediator mediator,
            IEmailService emailService,
            IUserManager userManager,
            IAuditLogService auditLogService)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _emailService = emailService;
            _userManager = userManager;
            _auditLogService = auditLogService;
        }

        public async Task<Result<string>> Handle(AdminCancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = (await _unitOfWork.Repository<OrderEntity>().GetAllWithIncludesAsync(
                o => o.Id == request.OrderId,
                o => o,
                o => o.Car!)).FirstOrDefault();

            if (order == null)
                return Result<string>.Fail("Order not found.");

            if (order.Status == OrderStatus.Cancelled)
                return Result<string>.Fail("Order is already cancelled.");

            bool wasOutOfStock = order.Car != null && order.Car.Quantity == 0;

            var oldStatus = order.Status;

            if (order.Car != null)
                order.Car.Quantity += order.Quantity;

            order.Cancel();
            _unitOfWork.Repository<OrderEntity>().Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (wasOutOfStock && order.Car != null)
                await _mediator.Send(new TriggerStockAlertsCommand(order.Car.Id));

            await _auditLogService.LogAsync("Order", "AdminCancel",
                null, null,
                $"Admin cancelled order #{request.OrderId} for user {order.UserId} (car: '{order.Car?.Title}')",
                entityId: request.OrderId,
                oldValues: JsonSerializer.Serialize(new { Status = oldStatus.ToString() }),
                newValues: JsonSerializer.Serialize(new { Status = "Cancelled" }));

            if (order.UserId != null)
            {
                await _mediator.Send(new CreateNotificationCommand(order.UserId,
                    $"Your order for {order.Car?.Title ?? "a car"} was cancelled by admin.",
                    "/Order/MyOrders"));

                try
                {
                    var user = await _userManager.FindByIdAsync(order.UserId);
                    if (user?.Email != null)
                    {
                        await _emailService.SendEmailAsync(
                            user.Email,
                            "Order Cancelled - CarShop",
                            $"<h2>Order Cancelled</h2><p>Your order for <strong>{order.Car?.Title ?? "a car"}</strong> has been cancelled by our team. Please contact support if you have questions.</p>"
                        );
                    }
                }
                catch { /* ignore email failures */ }
            }

            return Result<string>.Ok(null, "Order cancelled successfully.");
        }
    }
}
