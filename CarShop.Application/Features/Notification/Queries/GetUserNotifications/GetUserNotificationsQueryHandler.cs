using CarShop.Application.DTOs.Notification;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Notification.Queries.GetUserNotifications
{
    public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, Result<IEnumerable<AppNotificationDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public GetUserNotificationsQueryHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<IEnumerable<AppNotificationDto>>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var notifications = await _unitOfWork.Repository<CarShop.Domain.Entities.AppNotification>().GetAllAsync(
                n => n.UserId == userId,
                n => new AppNotificationDto
                {
                    Id = n.Id,
                    Message = n.Message,
                    Link = n.Link,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                });

            var ordered = notifications.OrderByDescending(n => n.CreatedAt);
            return Result<IEnumerable<AppNotificationDto>>.Ok(ordered);
        }
    }
}
