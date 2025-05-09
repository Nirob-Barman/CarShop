using CarShop.Application.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> Buy(int carId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _orderService.PlaceOrderAsync(userId!, carId);
                TempData["SuccessMessage"] = "Car purchased successfully!";
                return RedirectToAction("Details", "Home", new { id = carId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Details", "Home", new { id = carId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _orderService.CancelOrderAsync(orderId, userId!);
                TempData["SuccessMessage"] = "Order canceled successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("MyOrders");
        }



        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _orderService.GetOrdersByUserIdAsync(userId!);
            return View(orders);
        }
    }
}
