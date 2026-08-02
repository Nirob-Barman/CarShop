using CarShop.Application.DTOs.StockAlert;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using StockAlertEntity = CarShop.Domain.Entities.StockAlert;

namespace CarShop.Application.Features.StockAlert.Queries.GetUserAlerts
{
    public class GetUserAlertsQueryHandler : IRequestHandler<GetUserAlertsQuery, Result<IEnumerable<StockAlertDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public GetUserAlertsQueryHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<IEnumerable<StockAlertDto>>> Handle(GetUserAlertsQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var alerts = await _unitOfWork.Repository<StockAlertEntity>().GetAllWithIncludesAsync(
                predicate: s => s.UserId == userId && !s.IsTriggered,
                selector: s => s,
                s => s.Car!
            );

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
