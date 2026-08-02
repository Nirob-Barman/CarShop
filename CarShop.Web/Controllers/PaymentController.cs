using CarShop.Application.Features.Car.Queries.GetCarById;
using CarShop.Application.Features.Payment.Commands.HandlePaymentCancel;
using CarShop.Application.Features.Payment.Commands.HandlePaymentSuccess;
using CarShop.Application.Features.Payment.Commands.InitiatePayment;
using CarShop.Application.Features.PaymentGateway.Queries.GetActiveGateways;
using CarShop.Application.Features.PromoCode.Queries.ValidatePromoCode;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IMediator              _mediator;

        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout(int carId, string? promoCode)
        {
            var carResult = await _mediator.Send(new GetCarByIdQuery(carId));
            if (!carResult.Success || carResult.Data == null)
            {
                TempData["ErrorMessage"] = "Car not found.";
                return RedirectToAction("Index", "Home");
            }

            var car = carResult.Data;
            if (car.Quantity <= 0)
            {
                TempData["ErrorMessage"] = "This car is out of stock.";
                return RedirectToAction("Details", "Car", new { id = carId });
            }

            var gateways = (await _mediator.Send(new GetActiveGatewaysQuery())).Data?.ToList() ?? [];

            decimal finalPrice = car.Price;
            decimal discount   = 0;

            if (!string.IsNullOrWhiteSpace(promoCode))
            {
                var promoResult = await _mediator.Send(new ValidatePromoCodeQuery(promoCode));
                if (promoResult.Success && promoResult.Data != null)
                {
                    discount   = car.Price * (promoResult.Data.DiscountPercent / 100m);
                    if (promoResult.Data.MaxDiscountAmount.HasValue && discount > promoResult.Data.MaxDiscountAmount.Value)
                        discount = promoResult.Data.MaxDiscountAmount.Value;
                    finalPrice = car.Price - discount;
                }
            }

            ViewBag.Car        = car;
            ViewBag.PromoCode  = promoCode;
            ViewBag.FinalPrice = finalPrice;
            ViewBag.Discount   = discount;
            ViewBag.Gateways   = gateways;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Initiate(int carId, string? promoCode, int gatewayId)
        {
            var successUrl = Url.Action("Success", "Payment", null, Request.Scheme)!;
            var cancelUrl  = Url.Action("Cancel",  "Payment", null, Request.Scheme)!;

            var result = await _mediator.Send(new InitiatePaymentCommand(
                carId, gatewayId, promoCode, successUrl, cancelUrl));

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "Payment initiation failed.";
                return RedirectToAction("Checkout", new { carId, promoCode });
            }

            return Redirect(result.Data!);
        }

        [HttpGet]
        public async Task<IActionResult> Success(int txId, string gateway)
        {
            var result = await _mediator.Send(new HandlePaymentSuccessCommand(txId, gateway));
            ViewBag.IsSuccess = result.Success;
            ViewBag.Message   = result.Success
                ? "Payment confirmed! Your order is now active."
                : (result.Errors?.FirstOrDefault() ?? "Could not verify payment.");
            return View();
        }

        // SSLCommerz posts back to the success URL — handle the IPN/redirect callback
        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Success(int txId, string gateway, [FromForm] string? val_id)
        {
            var result = await _mediator.Send(new HandlePaymentSuccessCommand(txId, gateway, val_id));
            ViewBag.IsSuccess = result.Success;
            ViewBag.Message   = result.Success
                ? "Payment confirmed! Your order is now active."
                : (result.Errors?.FirstOrDefault() ?? "Could not verify payment.");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Cancel(int txId)
        {
            await _mediator.Send(new HandlePaymentCancelCommand(txId));
            return View();
        }
    }
}
