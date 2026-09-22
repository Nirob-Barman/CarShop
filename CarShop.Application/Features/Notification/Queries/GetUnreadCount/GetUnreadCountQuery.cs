using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Notification.Queries.GetUnreadCount
{
    public record GetUnreadCountQuery : IRequest<Result<int>>;
}
