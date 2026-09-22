using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StockAlertEntity = CarShop.Domain.Entities.StockAlert;

namespace CarShop.Application.Features.StockAlert.Commands.SubscribeStockAlert
{
    public class SubscribeStockAlertCommandHandler : IRequestHandler<SubscribeStockAlertCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public SubscribeStockAlertCommandHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(SubscribeStockAlertCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var exists = await _context.StockAlerts.AnyAsync(s => s.UserId == userId && s.CarId == request.CarId && !s.IsTriggered);
            if (exists)
                return Result<string>.Fail("You are already subscribed to stock alerts for this car.");

            var car = await _context.Cars.FindAsync(request.CarId);
            if (car == null)
                return Result<string>.Fail("Car not found.");

            var alert = StockAlertEntity.Create(userId, request.CarId);

            await _context.StockAlerts.AddAsync(alert);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "You will be notified when this car is back in stock.");
        }
    }
}
