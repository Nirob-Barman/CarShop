using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Notification.Commands.CreateNotification
{
    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;

        public CreateNotificationCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<string>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = new CarShop.Domain.Entities.AppNotification
            {
                UserId = request.UserId,
                Message = request.Message,
                Link = request.Link,
                CreatedAt = DateTime.UtcNow
            };

            await _context.AppNotifications.AddAsync(notification);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "Notification created.");
        }
    }
}
