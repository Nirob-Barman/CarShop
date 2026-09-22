using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Notification.Queries.GetUnreadCount
{
    public class GetUnreadCountQueryHandler : IRequestHandler<GetUnreadCountQuery, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetUnreadCountQueryHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<int>> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var count = await _context.AppNotifications.CountAsync(
                n => n.UserId == userId && !n.IsRead, cancellationToken);
            return Result<int>.Ok(count);
        }
    }
}
