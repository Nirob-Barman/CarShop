using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Notification.Queries.GetUnreadCount
{
    public class GetUnreadCountQuery : IRequest<Result<int>>
    {
    }
}
