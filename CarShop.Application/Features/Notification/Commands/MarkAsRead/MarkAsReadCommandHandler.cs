using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Notification.Commands.MarkAsRead
{
    public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public MarkAsReadCommandHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var notification = await _context.AppNotifications.FirstOrDefaultAsync(
                n => n.Id == request.NotificationId && n.UserId == userId);

            if (notification == null)
                return Result<string>.Fail("Notification not found.");

            notification.MarkAsRead();
            _context.AppNotifications.Update(notification);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "Marked as read.");
        }
    }
}
