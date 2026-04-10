using CarShop.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize]
    public class StockAlertController : UserDashboardController
    {
        private readonly IStockAlertService _stockAlertService;

        public StockAlertController(IStockAlertService stockAlertService, IUserService userService, IUserContextService userContextService)
            : base(userService, userContextService)
        {
            _stockAlertService = stockAlertService;
        }

        public async Task<IActionResult> MyAlerts()
        {
            var result = await _stockAlertService.GetUserAlertsAsync();
            return View(result.Data ?? Enumerable.Empty<CarShop.Application.DTOs.StockAlert.StockAlertDto>());
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(int carId)
        {
            var result = await _stockAlertService.SubscribeAsync(carId);
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "Could not subscribe.";
            return RedirectToAction("Details", "Car", new { id = carId });
        }

        [HttpPost]
        public async Task<IActionResult> Unsubscribe(int carId)
        {
            var result = await _stockAlertService.UnsubscribeAsync(carId);
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "Could not unsubscribe.";

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
                return Redirect(referer);
            return RedirectToAction("MyAlerts");
        }
    }
}
