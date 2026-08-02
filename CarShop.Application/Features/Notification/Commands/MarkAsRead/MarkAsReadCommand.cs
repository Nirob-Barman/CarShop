using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Notification.Commands.MarkAsRead
{
    public class MarkAsReadCommand : IRequest<Result<string>>
    {
        public int NotificationId { get; set; }

        public MarkAsReadCommand(int notificationId)
        {
            NotificationId = notificationId;
        }
    }
}
