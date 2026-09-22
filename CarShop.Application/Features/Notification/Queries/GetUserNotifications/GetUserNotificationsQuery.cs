using CarShop.Application.DTOs.Notification;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Notification.Queries.GetUserNotifications
{
    public record GetUserNotificationsQuery : IRequest<Result<IEnumerable<AppNotificationDto>>>;
}
