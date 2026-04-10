using CarShop.Application.Interfaces;
using CarShop.Web.ViewModels.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize]
    public class OrderController : UserDashboardController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService, IUserService userService, IUserContextService userContextService)
            : base(userService, userContextService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            try
            {
                await _orderService.CancelOrderAsync(orderId);
                TempData["SuccessMessage"] = "Order canceled successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("MyOrders");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _orderService.GetOrderByIdAsync(id);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("MyOrders");
            }
            return View(OrderMapper.ToViewModel(result.Data!));
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            await _orderService.ExpireStalePendingOrdersAsync();
            var orders = await _orderService.GetOrdersByUserIdAsync();
            var ordersVm = OrderMapper.ToViewModels(orders.Data!);
            return View(ordersVm);
        }
    }
}
