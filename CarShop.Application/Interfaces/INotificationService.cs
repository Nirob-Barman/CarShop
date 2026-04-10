using CarShop.Application.DTOs.Notification;
using CarShop.Application.Wrappers;

namespace CarShop.Application.Interfaces
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(string userId, string message, string? link = null); // userId kept — called by other services for specific users
        Task<Result<IEnumerable<AppNotificationDto>>> GetUserNotificationsAsync();
        Task<Result<int>> GetUnreadCountAsync();
        Task<Result<string>> MarkAsReadAsync(int notificationId);
        Task<Result<string>> MarkAllAsReadAsync();
    }
}
