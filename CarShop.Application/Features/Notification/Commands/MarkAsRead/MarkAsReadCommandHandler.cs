using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Notification.Commands.MarkAsRead
{
    public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public MarkAsReadCommandHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var notification = await _unitOfWork.Repository<CarShop.Domain.Entities.AppNotification>().FirstOrDefaultAsync(
                n => n.Id == request.NotificationId && n.UserId == userId);

            if (notification == null)
                return Result<string>.Fail("Notification not found.");

            notification.IsRead = true;
            _unitOfWork.Repository<CarShop.Domain.Entities.AppNotification>().Update(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "Marked as read.");
        }
    }
}
