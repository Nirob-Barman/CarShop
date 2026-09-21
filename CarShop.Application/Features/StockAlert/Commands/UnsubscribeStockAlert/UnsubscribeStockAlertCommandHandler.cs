using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.StockAlert.Commands.UnsubscribeStockAlert
{
    public class UnsubscribeStockAlertCommandHandler : IRequestHandler<UnsubscribeStockAlertCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public UnsubscribeStockAlertCommandHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(UnsubscribeStockAlertCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var alert = await _context.StockAlerts.FirstOrDefaultAsync(
                s => s.UserId == userId && s.CarId == request.CarId && !s.IsTriggered);

            if (alert == null)
                return Result<string>.Fail("No active stock alert found for this car.");

            _context.StockAlerts.Remove(alert);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "Stock alert removed.");
        }
    }
}
