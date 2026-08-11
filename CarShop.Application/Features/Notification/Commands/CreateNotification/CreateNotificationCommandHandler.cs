using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Notification.Commands.CreateNotification
{
    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateNotificationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

            await _unitOfWork.Repository<CarShop.Domain.Entities.AppNotification>().AddAsync(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "Notification created.");
        }
    }
}
