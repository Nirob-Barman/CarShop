using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Notification.Commands.MarkAllAsRead
{
    public class MarkAllAsReadCommand : IRequest<Result<string>>
    {
    }
}
