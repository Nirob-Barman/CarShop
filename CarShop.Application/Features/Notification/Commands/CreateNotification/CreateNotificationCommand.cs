using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Notification.Commands.CreateNotification
{
    public class CreateNotificationCommand : IRequest<Result<string>>
    {
        public string UserId { get; set; }
        public string Message { get; set; }
        public string? Link { get; set; }

        public CreateNotificationCommand(string userId, string message, string? link = null)
        {
            UserId = userId;
            Message = message;
            Link = link;
        }
    }
}
