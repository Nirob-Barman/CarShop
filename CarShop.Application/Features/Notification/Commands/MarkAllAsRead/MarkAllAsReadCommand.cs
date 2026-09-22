using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Notification.Commands.MarkAllAsRead
{
    public record MarkAllAsReadCommand : IRequest<Result<string>>;
}
