using CarShop.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize]
    public class NotificationController : UserDashboardController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService, IUserService userService, IUserContextService userContextService)
            : base(userService, userContextService)
        {
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _notificationService.GetUserNotificationsAsync();
            return View(result.Data ?? Enumerable.Empty<CarShop.Application.DTOs.Notification.AppNotificationDto>());
        }

        [HttpGet]
        public async Task<IActionResult> UnreadCount()
        {
            var result = await _notificationService.GetUnreadCountAsync();
            return Json(new { count = result.Data });
        }

        [HttpPost]
        public async Task<IActionResult> MarkRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllRead()
        {
            await _notificationService.MarkAllAsReadAsync();
            TempData["SuccessMessage"] = "All notifications marked as read.";
            return RedirectToAction("Index");
        }
    }
}
