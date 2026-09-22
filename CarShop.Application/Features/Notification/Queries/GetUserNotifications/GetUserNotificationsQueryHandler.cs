using CarShop.Application.DTOs.Notification;
using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Notification.Queries.GetUserNotifications
{
    public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, Result<IEnumerable<AppNotificationDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetUserNotificationsQueryHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<IEnumerable<AppNotificationDto>>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var notifications = await _context.AppNotifications.AsNoTracking()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new AppNotificationDto
                {
                    Id = n.Id,
                    Message = n.Message,
                    Link = n.Link,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                }).ToListAsync(cancellationToken);

            return Result<IEnumerable<AppNotificationDto>>.Ok(notifications);
        }
    }
}
