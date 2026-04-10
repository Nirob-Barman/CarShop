using CarShop.Application.DTOs.PromoCode;
using CarShop.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PromoCodeController : Controller
    {
        private readonly IPromoCodeService _promoCodeService;

        public PromoCodeController(IPromoCodeService promoCodeService)
        {
            _promoCodeService = promoCodeService;
        }

        // ── Admin actions ──────────────────────────────────────────

        public async Task<IActionResult> Index()
        {
            var result = await _promoCodeService.GetAllCodesAsync();
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

            var result = await _promoCodeService.CreateCodeAsync(dto);
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
            var result = await _promoCodeService.GetByIdAsync(id);
            if (!result.Success) return RedirectToAction("Index");
            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PromoCodeDto dto)
        {
            var result = await _promoCodeService.UpdateCodeAsync(id, dto);
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
            var result = await _promoCodeService.ToggleActiveAsync(id);
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
            var result = await _promoCodeService.DeleteCodeAsync(id);
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
            var result = await _promoCodeService.GetAllActiveCodesAsync();
            return View(result.Data ?? Enumerable.Empty<PromoCodeDto>());
        }
    }
}
