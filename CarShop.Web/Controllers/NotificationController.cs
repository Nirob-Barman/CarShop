using CarShop.Application.Features.Notification.Commands.MarkAllAsRead;
using CarShop.Application.Features.Notification.Commands.MarkAsRead;
using CarShop.Application.Features.Notification.Queries.GetUnreadCount;
using CarShop.Application.Features.Notification.Queries.GetUserNotifications;
using CarShop.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize]
    public class NotificationController : UserDashboardController
    {
        private readonly IMediator _mediator;

        public NotificationController(IMediator mediator, IUserContextService userContextService)
            : base(mediator, userContextService)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _mediator.Send(new GetUserNotificationsQuery());
            return View(result.Data ?? Enumerable.Empty<CarShop.Application.DTOs.Notification.AppNotificationDto>());
        }

        [HttpGet]
        public async Task<IActionResult> UnreadCount()
        {
            var result = await _mediator.Send(new GetUnreadCountQuery());
            return Json(new { count = result.Data });
        }

        [HttpPost]
        public async Task<IActionResult> MarkRead(int id)
        {
            await _mediator.Send(new MarkAsReadCommand(id));
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllRead()
        {
            await _mediator.Send(new MarkAllAsReadCommand());
            TempData["SuccessMessage"] = "All notifications marked as read.";
            return RedirectToAction("Index");
        }
    }
}
