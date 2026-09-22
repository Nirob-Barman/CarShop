using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
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
            var notification = AppNotification.Create(request.UserId, request.Message, request.Link);

            await _context.AppNotifications.AddAsync(notification);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "Notification created.");
        }
    }
}
