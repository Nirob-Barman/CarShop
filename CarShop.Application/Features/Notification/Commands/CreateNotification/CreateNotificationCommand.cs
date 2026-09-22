using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Notification.Commands.CreateNotification
{
    public record CreateNotificationCommand(
        string UserId,
        string Message,
        string? Link = null
    ) : IRequest<Result<string>>;
}
