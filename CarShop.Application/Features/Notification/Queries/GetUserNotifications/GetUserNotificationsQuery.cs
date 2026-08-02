using CarShop.Application.DTOs.Notification;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Notification.Queries.GetUserNotifications
{
    public class GetUserNotificationsQuery : IRequest<Result<IEnumerable<AppNotificationDto>>>
    {
    }
}
