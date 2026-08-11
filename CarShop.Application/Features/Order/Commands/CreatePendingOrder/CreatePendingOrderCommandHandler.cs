using CarShop.Application.Features.PromoCode.Commands.IncrementPromoCodeUsage;
using CarShop.Application.Features.PromoCode.Queries.ValidatePromoCode;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using System.Text.Json;
using CarShop.Domain.Entities;
using CarEntity = CarShop.Domain.Entities.Car;
using OrderEntity = CarShop.Domain.Entities.Order;

namespace CarShop.Application.Features.Order.Commands.CreatePendingOrder
{
    public class CreatePendingOrderCommandHandler : IRequestHandler<CreatePendingOrderCommand, Result<(int OrderId, decimal FinalPrice, string CarTitle)>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public CreatePendingOrderCommandHandler(
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

        public async Task<Result<(int OrderId, decimal FinalPrice, string CarTitle)>> Handle(CreatePendingOrderCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;

            var car = await _unitOfWork.Repository<CarEntity>().GetByIdAsync(request.CarId);
            if (car == null) return Result<(int, decimal, string)>.Fail("Car not found.");

            // Cancel any existing pending order for this user + car to prevent stock double-hold
            var existingPending = await _unitOfWork.Repository<OrderEntity>().FirstOrDefaultAsync(
                o => o.UserId == userId && o.CarId == request.CarId && o.Status == OrderStatus.Pending);
            if (existingPending != null)
            {
                existingPending.Cancel();
                car.Quantity += existingPending.Quantity;  // restore the previously held stock
                _unitOfWork.Repository<OrderEntity>().Update(existingPending);
            }

            if (car.Quantity <= 0) return Result<(int, decimal, string)>.Fail("Car is out of stock.");

            decimal discountAmount = 0;
            decimal finalPrice = car.Price;
            string? appliedCode = null;
            int? promoCodeId = null;

            if (!string.IsNullOrWhiteSpace(request.PromoCode))
            {
                var promoResult = await _mediator.Send(new ValidatePromoCodeQuery(request.PromoCode));
                if (promoResult.Success && promoResult.Data != null)
                {
                    var promo = promoResult.Data;
                    discountAmount = car.Price * (promo.DiscountPercent / 100m);
                    if (promo.MaxDiscountAmount.HasValue && discountAmount > promo.MaxDiscountAmount.Value)
                        discountAmount = promo.MaxDiscountAmount.Value;
                    finalPrice = car.Price - discountAmount;
                    appliedCode = request.PromoCode.ToUpper();
                    promoCodeId = promo.PromoCodeId;
                }
            }

            car.Quantity--;

            var order = new OrderEntity
            {
                UserId         = userId,
                CarId          = request.CarId,
                OrderedAt      = DateTime.UtcNow,
                Quantity       = 1,
                PromoCode      = appliedCode,
                DiscountAmount = discountAmount,
                FinalPrice     = finalPrice
            };

            await _unitOfWork.Repository<OrderEntity>().AddAsync(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (promoCodeId.HasValue)
                await _mediator.Send(new IncrementPromoCodeUsageCommand(promoCodeId.Value));

            await _auditLogService.LogAsync("Order", "PendingCreated",
                userId, null,
                $"Pending order created for car '{car.Title}'",
                entityId: order.Id,
                newValues: JsonSerializer.Serialize(new
                {
                    order.CarId, CarTitle = car.Title,
                    order.FinalPrice, order.DiscountAmount,
                    order.PromoCode, Status = order.Status.ToString()
                }));

            return Result<(int, decimal, string)>.Ok((order.Id, finalPrice, car.Title ?? "Car"));
        }
    }
}
