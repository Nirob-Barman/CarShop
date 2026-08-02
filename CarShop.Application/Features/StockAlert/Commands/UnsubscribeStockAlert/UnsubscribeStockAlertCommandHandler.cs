using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using StockAlertEntity = CarShop.Domain.Entities.StockAlert;

namespace CarShop.Application.Features.StockAlert.Commands.UnsubscribeStockAlert
{
    public class UnsubscribeStockAlertCommandHandler : IRequestHandler<UnsubscribeStockAlertCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public UnsubscribeStockAlertCommandHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(UnsubscribeStockAlertCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var alert = await _unitOfWork.Repository<StockAlertEntity>().FirstOrDefaultAsync(
                s => s.UserId == userId && s.CarId == request.CarId && !s.IsTriggered);

            if (alert == null)
                return Result<string>.Fail("No active stock alert found for this car.");

            _unitOfWork.Repository<StockAlertEntity>().Remove(alert);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "Stock alert removed.");
        }
    }
}
