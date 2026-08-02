using CarShop.Application.Features.StockAlert.Commands.SubscribeStockAlert;
using CarShop.Application.Features.StockAlert.Commands.UnsubscribeStockAlert;
using CarShop.Application.Features.StockAlert.Queries.GetUserAlerts;
using CarShop.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize]
    public class StockAlertController : UserDashboardController
    {
        private readonly IMediator _mediator;

        public StockAlertController(IMediator mediator, IUserContextService userContextService)
            : base(mediator, userContextService)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> MyAlerts()
        {
            var result = await _mediator.Send(new GetUserAlertsQuery());
            return View(result.Data ?? Enumerable.Empty<CarShop.Application.DTOs.StockAlert.StockAlertDto>());
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(int carId)
        {
            var result = await _mediator.Send(new SubscribeStockAlertCommand(carId));
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "Could not subscribe.";
            return RedirectToAction("Details", "Car", new { id = carId });
        }

        [HttpPost]
        public async Task<IActionResult> Unsubscribe(int carId)
        {
            var result = await _mediator.Send(new UnsubscribeStockAlertCommand(carId));
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
