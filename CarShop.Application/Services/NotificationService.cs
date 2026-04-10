using CarShop.Application.DTOs.Notification;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;

namespace CarShop.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public NotificationService(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        // userId kept as param — called by other services to notify specific (possibly other) users
        public async Task CreateNotificationAsync(string userId, string message, string? link = null)
        {
            var notification = new AppNotification
            {
                UserId = userId,
                Message = message,
                Link = link,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<AppNotification>().AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Result<IEnumerable<AppNotificationDto>>> GetUserNotificationsAsync()
        {
            var userId = _userContextService.UserId!;
            var notifications = await _unitOfWork.Repository<AppNotification>().GetAllAsync(
                n => n.UserId == userId,
                n => new AppNotificationDto
                {
                    Id = n.Id,
                    Message = n.Message,
                    Link = n.Link,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                });

            var ordered = notifications.OrderByDescending(n => n.CreatedAt);
            return Result<IEnumerable<AppNotificationDto>>.Ok(ordered);
        }

        public async Task<Result<int>> GetUnreadCountAsync()
        {
            var userId = _userContextService.UserId!;
            var count = await _unitOfWork.Repository<AppNotification>().CountAsync(
                n => n.UserId == userId && !n.IsRead);
            return Result<int>.Ok(count);
        }

        public async Task<Result<string>> MarkAsReadAsync(int notificationId)
        {
            var userId = _userContextService.UserId!;
            var notification = await _unitOfWork.Repository<AppNotification>().FirstOrDefaultAsync(
                n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
                return Result<string>.Fail("Notification not found.");

            notification.IsRead = true;
            _unitOfWork.Repository<AppNotification>().Update(notification);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Ok(null, "Marked as read.");
        }

        public async Task<Result<string>> MarkAllAsReadAsync()
        {
            var userId = _userContextService.UserId!;
            var notifications = await _unitOfWork.Repository<AppNotification>().GetAllAsync(
                n => n.UserId == userId && !n.IsRead,
                n => n);

            var list = notifications.ToList();
            foreach (var n in list)
                n.IsRead = true;

            _unitOfWork.Repository<AppNotification>().UpdateRange(list);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Ok(null, "All notifications marked as read.");
        }
    }
}
