using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Notification.Commands.MarkAsRead
{
    public record MarkAsReadCommand(int NotificationId) : IRequest<Result<string>>;
}
