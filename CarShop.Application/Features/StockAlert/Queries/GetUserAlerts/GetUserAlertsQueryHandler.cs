using CarShop.Application.DTOs.StockAlert;
using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.StockAlert.Queries.GetUserAlerts
{
    public class GetUserAlertsQueryHandler : IRequestHandler<GetUserAlertsQuery, Result<IEnumerable<StockAlertDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetUserAlertsQueryHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<IEnumerable<StockAlertDto>>> Handle(GetUserAlertsQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var alerts = await _context.StockAlerts.AsNoTracking()
                .Include(s => s.Car)
                .Where(s => s.UserId == userId && !s.IsTriggered)
                .ToListAsync(cancellationToken);

            var dtos = alerts.Select(s => new StockAlertDto
            {
                Id = s.Id,
                CarId = s.CarId,
                CarTitle = s.Car?.Title,
                CarImageUrl = s.Car?.ImageUrl,
                IsTriggered = s.IsTriggered,
                SubscribedAt = s.SubscribedAt,
                TriggeredAt = s.TriggeredAt
            });

            return Result<IEnumerable<StockAlertDto>>.Ok(dtos);
        }
    }
}
