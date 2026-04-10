using CarShop.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index(string? status, int page = 1)
        {
            var result = await _orderService.GetAllOrdersAsync(status, page, 20);
            ViewBag.Status = status;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = result.Data?.TotalPages ?? 1;
            return View(result.Data?.Items ?? Enumerable.Empty<CarShop.Application.DTOs.Order.OrderDto>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await _orderService.GetOrderByIdAdminAsync(id);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Index");
            }
            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int orderId)
        {
            var result = await _orderService.AdminCancelOrderAsync(orderId);
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "Could not cancel order.";
            return RedirectToAction("Index");
        }
    }
}
