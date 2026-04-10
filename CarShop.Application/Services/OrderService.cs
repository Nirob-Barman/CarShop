using CarShop.Application.DTOs.Order;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using System.Text.Json;

namespace CarShop.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IStockAlertService _stockAlertService;
        private readonly IPromoCodeService _promoCodeService;
        private readonly IEmailService _emailService;
        private readonly IUserManager _userManager;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public OrderService(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IStockAlertService stockAlertService,
            IPromoCodeService promoCodeService,
            IEmailService emailService,
            IUserManager userManager,
            IAuditLogService auditLogService,
            IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _stockAlertService = stockAlertService;
            _promoCodeService = promoCodeService;
            _emailService = emailService;
            _userManager = userManager;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task ExpireStalePendingOrdersAsync(int olderThanMinutes = 30)
        {
            var userId = _userContextService.UserId!;
            await ExpireStalePendingOrdersCoreAsync(userId, olderThanMinutes);
        }

        private async Task ExpireStalePendingOrdersCoreAsync(string userId, int olderThanMinutes = 30)
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-olderThanMinutes);
            var stale  = await _unitOfWork.Repository<Order>().GetAllWithIncludesAsync(
                predicate: o => o.UserId == userId && o.Status == "Pending" && o.OrderedAt < cutoff,
                selector:  o => o,
                o => o.Car!);

            foreach (var order in stale)
            {
                if (order.Car != null)
                {
                    order.Car.Quantity += order.Quantity;
                    _unitOfWork.Repository<Car>().Update(order.Car);
                }
                order.Status = "Cancelled";
                _unitOfWork.Repository<Order>().Update(order);
            }

            if (stale.Any())
                await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Result<(int OrderId, decimal FinalPrice, string CarTitle)>> CreatePendingOrderAsync(int carId, string? promoCode = null)
        {
            var userId = _userContextService.UserId!;
            return await CreatePendingOrderCoreAsync(userId, carId, promoCode);
        }

        private async Task<Result<(int OrderId, decimal FinalPrice, string CarTitle)>> CreatePendingOrderCoreAsync(string userId, int carId, string? promoCode = null)
        {
            var car = await _unitOfWork.Repository<Car>().GetByIdAsync(carId);
            if (car == null) return Result<(int, decimal, string)>.Fail("Car not found.");

            // Cancel any existing pending order for this user + car to prevent stock double-hold
            var existingPending = await _unitOfWork.Repository<Order>().FirstOrDefaultAsync(
                o => o.UserId == userId && o.CarId == carId && o.Status == "Pending");
            if (existingPending != null)
            {
                existingPending.Status = "Cancelled";
                car.Quantity += existingPending.Quantity;  // restore the previously held stock
                _unitOfWork.Repository<Order>().Update(existingPending);
            }

            if (car.Quantity <= 0) return Result<(int, decimal, string)>.Fail("Car is out of stock.");

            decimal discountAmount = 0;
            decimal finalPrice = car.Price;
            string? appliedCode = null;
            int? promoCodeId = null;

            if (!string.IsNullOrWhiteSpace(promoCode))
            {
                var promoResult = await _promoCodeService.ValidateCodeAsync(promoCode);
                if (promoResult.Success && promoResult.Data != null)
                {
                    var promo = promoResult.Data;
                    discountAmount = car.Price * (promo.DiscountPercent / 100m);
                    if (promo.MaxDiscountAmount.HasValue && discountAmount > promo.MaxDiscountAmount.Value)
                        discountAmount = promo.MaxDiscountAmount.Value;
                    finalPrice = car.Price - discountAmount;
                    appliedCode = promoCode.ToUpper();
                    promoCodeId = promo.PromoCodeId;
                }
            }

            car.Quantity--;

            var order = new Order
            {
                UserId        = userId,
                CarId         = carId,
                OrderedAt     = DateTime.UtcNow,
                Quantity      = 1,
                Status        = "Pending",
                PromoCode     = appliedCode,
                DiscountAmount = discountAmount,
                FinalPrice    = finalPrice
            };

            await _unitOfWork.Repository<Order>().AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            if (promoCodeId.HasValue)
                await _promoCodeService.IncrementUsageAsync(promoCodeId.Value);

            await _auditLogService.LogAsync("Order", "PendingCreated",
                userId, null,
                $"Pending order created for car '{car.Title}'",
                entityId: order.Id,
                newValues: JsonSerializer.Serialize(new
                {
                    order.CarId, CarTitle = car.Title,
                    order.FinalPrice, order.DiscountAmount,
                    order.PromoCode, order.Status
                }));

            return Result<(int, decimal, string)>.Ok((order.Id, finalPrice, car.Title ?? "Car"));
        }

        public async Task SetOrderGatewayAsync(int orderId, int paymentGatewayId)
        {
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId);
            if (order == null) return;
            order.PaymentGatewayId = paymentGatewayId;
            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Result<string>> MarkOrderAsPaidAsync(int orderId)
        {
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId);
            if (order == null) return Result<string>.Fail("Order not found.");
            if (order.Status == "Confirmed") return Result<string>.Ok(null, "Already confirmed.");

            var oldStatus = order.Status;
            order.Status = "Confirmed";
            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("Order", "Confirmed",
                order.UserId, null,
                $"Payment confirmed for order #{orderId}",
                entityId: orderId,
                oldValues: JsonSerializer.Serialize(new { Status = oldStatus }),
                newValues: JsonSerializer.Serialize(new { Status = "Confirmed" }));

            return Result<string>.Ok(null, "Order confirmed.");
        }

        public async Task<Result<string>> CancelPendingOrderByIdAsync(int orderId)
        {
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId);
            return await CancelPendingOrderCoreAsync(order);
        }

        private async Task<Result<string>> CancelPendingOrderCoreAsync(Order? order)
        {
            if (order == null || order.Status != "Pending")
                return Result<string>.Ok(null, "Nothing to cancel.");

            var car = await _unitOfWork.Repository<Car>().GetByIdAsync(order.CarId);
            if (car != null) { car.Quantity += order.Quantity; _unitOfWork.Repository<Car>().Update(car); }

            var oldStatus = order.Status;
            order.Status = "Cancelled";
            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("Order", "Cancel",
                order.UserId, null,
                $"Pending order #{order.Id} cancelled",
                entityId: order.Id,
                oldValues: JsonSerializer.Serialize(new { Status = oldStatus }),
                newValues: JsonSerializer.Serialize(new { Status = "Cancelled" }));

            return Result<string>.Ok(null, "Order cancelled.");
        }

        public async Task<Result<string>> PlaceOrderAsync(int carId, string? promoCode = null)
        {
            var userId = _userContextService.UserId!;
            return await PlaceOrderCoreAsync(userId, carId, promoCode);
        }

        private async Task<Result<string>> PlaceOrderCoreAsync(string userId, int carId, string? promoCode = null)
        {
            var car = await _unitOfWork.Repository<Car>().GetByIdAsync(carId);

            if (car == null)
                return Result<string>.Fail("Car not found.");

            if (car.Quantity <= 0)
                return Result<string>.Fail("Car is out of stock.");

            decimal discountAmount = 0;
            decimal finalPrice = car.Price;
            string? appliedCode = null;
            int? promoCodeId = null;

            if (!string.IsNullOrWhiteSpace(promoCode))
            {
                var promoResult = await _promoCodeService.ValidateCodeAsync(promoCode);
                if (promoResult.Success && promoResult.Data != null)
                {
                    var promo = promoResult.Data;
                    discountAmount = car.Price * (promo.DiscountPercent / 100m);
                    if (promo.MaxDiscountAmount.HasValue && discountAmount > promo.MaxDiscountAmount.Value)
                        discountAmount = promo.MaxDiscountAmount.Value;

                    finalPrice = car.Price - discountAmount;
                    appliedCode = promoCode.ToUpper();
                    promoCodeId = promo.PromoCodeId;
                }
            }

            car.Quantity--;

            var order = new Order
            {
                UserId = userId,
                CarId = carId,
                OrderedAt = DateTime.UtcNow,
                Quantity = 1,
                Status = "Confirmed",
                PromoCode = appliedCode,
                DiscountAmount = discountAmount,
                FinalPrice = finalPrice
            };

            await _unitOfWork.Repository<Order>().AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            if (promoCodeId.HasValue)
                await _promoCodeService.IncrementUsageAsync(promoCodeId.Value);

            await _auditLogService.LogAsync("Order", "Placed",
                userId, null,
                $"Order placed for car '{car.Title}'",
                entityId: order.Id,
                newValues: JsonSerializer.Serialize(new
                {
                    order.CarId, CarTitle = car.Title,
                    order.FinalPrice, order.DiscountAmount,
                    order.PromoCode, order.Status
                }));

            await _notificationService.CreateNotificationAsync(userId,
                $"Your order for {car.Title} has been placed successfully.",
                "/Order/MyOrders");

            // Email notification
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user?.Email != null)
                {
                    await _emailService.SendEmailAsync(
                        user.Email,
                        "Order Confirmed - CarShop",
                        $"<h2>Order Confirmed!</h2><p>Your order for <strong>{car.Title}</strong> has been placed.</p><p>Total: <strong>${finalPrice:F2}</strong>{(discountAmount > 0 ? $" (Saved: ${discountAmount:F2})" : "")}</p>"
                    );
                }
            }
            catch { /* ignore email failures */ }

            return Result<string>.Ok(null, "Order placed successfully.");
        }

        public async Task<Result<IEnumerable<OrderDto>>> GetOrdersByUserIdAsync()
        {
            var userId = _userContextService.UserId!;
            return await GetOrdersByUserIdCoreAsync(userId);
        }

        private async Task<Result<IEnumerable<OrderDto>>> GetOrdersByUserIdCoreAsync(string userId)
        {
            var orders = await _unitOfWork.Repository<Order>().GetAllWithIncludesAsync(
                predicate: o => o.UserId == userId,
                selector: o => o,
                o => o.Car!
            );

            var dtos = orders.OrderByDescending(o => o.OrderedAt).Select(o => new OrderDto
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
                FinalPrice = o.FinalPrice > 0 ? o.FinalPrice : o.Car?.Price ?? 0
            });

            return Result<IEnumerable<OrderDto>>.Ok(dtos);
        }

        public async Task<Result<OrderDto>> GetOrderByIdAsync(int orderId)
        {
            var userId = _userContextService.UserId!;
            var order  = (await _unitOfWork.Repository<Order>().GetAllWithIncludesAsync(
                o => o.Id == orderId && o.UserId == userId,
                o => o,
                o => o.Car!)).FirstOrDefault();

            if (order == null)
                return Result<OrderDto>.Fail("Order not found.");

            var dto = new OrderDto
            {
                Id             = order.Id,
                UserId         = order.UserId!,
                CarId          = order.CarId,
                OrderedAt      = order.OrderedAt,
                Quantity       = order.Quantity,
                CarTitle       = order.Car?.Title ?? "N/A",
                CarPrice       = order.Car?.Price ?? 0,
                CarImageUrl    = order.Car?.ImageUrl,
                Status         = order.Status,
                PromoCode      = order.PromoCode,
                DiscountAmount = order.DiscountAmount,
                FinalPrice     = order.FinalPrice > 0 ? order.FinalPrice : order.Car?.Price ?? 0
            };

            return Result<OrderDto>.Ok(dto);
        }

        public async Task<Result<string>> CancelOrderAsync(int orderId)
        {
            var userId = _userContextService.UserId!;
            var order = (await _unitOfWork.Repository<Order>().GetAllWithIncludesAsync(
                o => o.Id == orderId && o.UserId == userId,
                o => o,
                o => o.Car!)).FirstOrDefault();

            if (order == null)
                return Result<string>.Fail("Order not found or you are not authorized to cancel it.");

            if (order.Status == "Cancelled")
                return Result<string>.Fail("Order is already cancelled.");

            bool wasOutOfStock = order.Car != null && order.Car.Quantity == 0;

            var oldStatus = order.Status;

            if (order.Car != null)
                order.Car.Quantity += order.Quantity;

            order.Status = "Cancelled";
            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("Order", "UserCancel",
                userId, null,
                $"User cancelled order #{orderId} for car '{order.Car?.Title}'",
                entityId: orderId,
                oldValues: JsonSerializer.Serialize(new { Status = oldStatus }),
                newValues: JsonSerializer.Serialize(new { Status = "Cancelled" }));

            if (wasOutOfStock && order.Car != null)
                await _stockAlertService.TriggerAlertsForCarAsync(order.Car.Id);

            await _notificationService.CreateNotificationAsync(userId,
                $"Your order for {order.Car?.Title ?? "a car"} has been cancelled.",
                "/Order/MyOrders");

            return Result<string>.Ok(null, "Order cancelled successfully.");
        }

        public async Task<Result<PagedResult<OrderDto>>> GetAllOrdersAsync(string? status = null, int page = 1, int pageSize = 20)
        {
            var allOrders = await _unitOfWork.Repository<Order>().GetAllWithIncludesAsync(
                predicate: o => status == null || o.Status == status,
                selector: o => o,
                o => o.Car!
            );

            var ordered = allOrders.OrderByDescending(o => o.OrderedAt).ToList();
            var totalCount = ordered.Count;

            var pagedItems = ordered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
                Page = page,
                PageSize = pageSize
            });
        }

        public async Task<int> GetCompletedOrdersCountAsync()
        {
            return await _unitOfWork.Repository<Order>()
                .CountAsync(o => o.Status == "Confirmed" || o.Status == "Paid");
        }

        public async Task<Result<OrderDto>> GetOrderByIdAdminAsync(int orderId)
        {
            var order = (await _unitOfWork.Repository<Order>().GetAllWithIncludesAsync(
                o => o.Id == orderId,
                o => o,
                o => o.Car!)).FirstOrDefault();

            if (order == null)
                return Result<OrderDto>.Fail("Order not found.");

            var user = await _userManager.FindByIdAsync(order.UserId ?? "");

            return Result<OrderDto>.Ok(new OrderDto
            {
                Id             = order.Id,
                UserId         = order.UserId!,
                CarId          = order.CarId,
                OrderedAt      = order.OrderedAt,
                Quantity       = order.Quantity,
                CarTitle       = order.Car?.Title ?? "N/A",
                CarPrice       = order.Car?.Price ?? 0,
                CarImageUrl    = order.Car?.ImageUrl,
                Status         = order.Status,
                PromoCode      = order.PromoCode,
                DiscountAmount = order.DiscountAmount,
                FinalPrice     = order.FinalPrice > 0 ? order.FinalPrice : order.Car?.Price ?? 0,
                UserEmail      = user?.Email,
                UserFullName   = user?.FullName
            });
        }

        public async Task<Result<string>> AdminCancelOrderAsync(int orderId)
        {
            var order = (await _unitOfWork.Repository<Order>().GetAllWithIncludesAsync(
                o => o.Id == orderId,
                o => o,
                o => o.Car!)).FirstOrDefault();

            if (order == null)
                return Result<string>.Fail("Order not found.");

            if (order.Status == "Cancelled")
                return Result<string>.Fail("Order is already cancelled.");

            bool wasOutOfStock = order.Car != null && order.Car.Quantity == 0;

            var oldStatus = order.Status;

            if (order.Car != null)
                order.Car.Quantity += order.Quantity;

            order.Status = "Cancelled";
            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.SaveChangesAsync();

            if (wasOutOfStock && order.Car != null)
                await _stockAlertService.TriggerAlertsForCarAsync(order.Car.Id);

            await _auditLogService.LogAsync("Order", "AdminCancel",
                null, null,
                $"Admin cancelled order #{orderId} for user {order.UserId} (car: '{order.Car?.Title}')",
                entityId: orderId,
                oldValues: JsonSerializer.Serialize(new { Status = oldStatus }),
                newValues: JsonSerializer.Serialize(new { Status = "Cancelled" }));

            if (order.UserId != null)
            {
                await _notificationService.CreateNotificationAsync(order.UserId,
                    $"Your order for {order.Car?.Title ?? "a car"} was cancelled by admin.",
                    "/Order/MyOrders");

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
