using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Notification.Commands.MarkAllAsRead
{
    public class MarkAllAsReadCommandHandler : IRequestHandler<MarkAllAsReadCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public MarkAllAsReadCommandHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(MarkAllAsReadCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var notifications = await _unitOfWork.Repository<CarShop.Domain.Entities.AppNotification>().GetAllAsync(
                n => n.UserId == userId && !n.IsRead,
                n => n);

            var list = notifications.ToList();
            foreach (var n in list)
                n.MarkAsRead();

            _unitOfWork.Repository<CarShop.Domain.Entities.AppNotification>().UpdateRange(list);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "All notifications marked as read.");
        }
    }
}
