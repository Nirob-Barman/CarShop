using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Notification.Commands.MarkAllAsRead
{
    public class MarkAllAsReadCommandHandler : IRequestHandler<MarkAllAsReadCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public MarkAllAsReadCommandHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(MarkAllAsReadCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var notifications = await _context.AppNotifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync(cancellationToken);

            var list = notifications.ToList();
            foreach (var n in list)
                n.MarkAsRead();

            _context.AppNotifications.UpdateRange(list);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "All notifications marked as read.");
        }
    }
}
