using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using CarShop.Domain.Entities;
using CarShop.Domain.Enums;
using CarEntity = CarShop.Domain.Entities.Car;
using OrderEntity = CarShop.Domain.Entities.Order;

namespace CarShop.Application.Features.Order.Commands.ExpireStalePendingOrders
{
    public class ExpireStalePendingOrdersCommandHandler : IRequestHandler<ExpireStalePendingOrdersCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public ExpireStalePendingOrdersCommandHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(ExpireStalePendingOrdersCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var cutoff = DateTime.UtcNow.AddMinutes(-request.OlderThanMinutes);
            var stale  = await _unitOfWork.Repository<OrderEntity>().GetAllWithIncludesAsync(
                predicate: o => o.UserId == userId && o.Status == OrderStatus.Pending && o.OrderedAt < cutoff,
                selector:  o => o,
                o => o.Car!);

            foreach (var order in stale)
            {
                if (order.Car != null)
                {
                    order.Car.RestoreStock(order.Quantity);
                    _unitOfWork.Repository<CarEntity>().Update(order.Car);
                }
                order.Cancel();
                _unitOfWork.Repository<OrderEntity>().Update(order);
            }

            if (stale.Any())
                await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "Stale pending orders expired.");
        }
    }
}
