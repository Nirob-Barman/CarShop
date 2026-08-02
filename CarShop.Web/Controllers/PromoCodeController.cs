using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Features.PromoCode.Commands.CreatePromoCode;
using CarShop.Application.Features.PromoCode.Commands.DeletePromoCode;
using CarShop.Application.Features.PromoCode.Commands.TogglePromoCodeActive;
using CarShop.Application.Features.PromoCode.Commands.UpdatePromoCode;
using CarShop.Application.Features.PromoCode.Queries.GetActivePromoCodes;
using CarShop.Application.Features.PromoCode.Queries.GetAllPromoCodes;
using CarShop.Application.Features.PromoCode.Queries.GetPromoCodeById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PromoCodeController : Controller
    {
        private readonly IMediator _mediator;

        public PromoCodeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ── Admin actions ──────────────────────────────────────────

        public async Task<IActionResult> Index()
        {
            var result = await _mediator.Send(new GetAllPromoCodesQuery());
            return View(result.Data ?? Enumerable.Empty<PromoCodeDto>());
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PromoCodeDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _mediator.Send(new CreatePromoCodeCommand(dto));
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "Failed to create promo code.";
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _mediator.Send(new GetPromoCodeByIdQuery(id));
            if (!result.Success) return RedirectToAction("Index");
            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PromoCodeDto dto)
        {
            var result = await _mediator.Send(new UpdatePromoCodeCommand(id, dto));
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            var result = await _mediator.Send(new TogglePromoCodeActiveCommand(id));
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeletePromoCodeCommand(id));
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault();
            return RedirectToAction("Index");
        }

        // ── Public deals page ──────────────────────────────────────

        [AllowAnonymous]
        public async Task<IActionResult> Deals()
        {
            var result = await _mediator.Send(new GetActivePromoCodesQuery());
            return View(result.Data ?? Enumerable.Empty<PromoCodeDto>());
        }
    }
}
