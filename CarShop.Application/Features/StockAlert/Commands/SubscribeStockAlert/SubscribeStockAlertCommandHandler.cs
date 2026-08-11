using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using CarEntity = CarShop.Domain.Entities.Car;
using StockAlertEntity = CarShop.Domain.Entities.StockAlert;

namespace CarShop.Application.Features.StockAlert.Commands.SubscribeStockAlert
{
    public class SubscribeStockAlertCommandHandler : IRequestHandler<SubscribeStockAlertCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public SubscribeStockAlertCommandHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(SubscribeStockAlertCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var exists = await _unitOfWork.Repository<StockAlertEntity>().AnyAsync(s => s.UserId == userId && s.CarId == request.CarId && !s.IsTriggered);
            if (exists)
                return Result<string>.Fail("You are already subscribed to stock alerts for this car.");

            var car = await _unitOfWork.Repository<CarEntity>().GetByIdAsync(request.CarId);
            if (car == null)
                return Result<string>.Fail("Car not found.");

            var alert = new StockAlertEntity
            {
                UserId = userId,
                CarId = request.CarId,
                SubscribedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<StockAlertEntity>().AddAsync(alert);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "You will be notified when this car is back in stock.");
        }
    }
}
