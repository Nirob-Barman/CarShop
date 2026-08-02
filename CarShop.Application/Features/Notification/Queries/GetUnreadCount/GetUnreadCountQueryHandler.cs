using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Notification.Queries.GetUnreadCount
{
    public class GetUnreadCountQueryHandler : IRequestHandler<GetUnreadCountQuery, Result<int>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public GetUnreadCountQueryHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<int>> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var count = await _unitOfWork.Repository<CarShop.Domain.Entities.AppNotification>().CountAsync(
                n => n.UserId == userId && !n.IsRead);
            return Result<int>.Ok(count);
        }
    }
}
