using CarShop.Application.Interfaces;
using CarShop.Web.ViewModels.Brand;
using CarShop.Web.ViewModels.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _brandService.GetAllBrandsAsync();
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message ?? "Failed to load brands.";
                return View(new List<BrandViewModel>());
            }
            return View(BrandMapper.ToViewModels(result.Data!));
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _brandService.CreateBrandAsync(BrandMapper.ToDto(model));
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message ?? result.Errors?.FirstOrDefault();
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _brandService.GetBrandByIdAsync(id);
            if (!result.Success || result.Data == null)
            {
                TempData["ErrorMessage"] = result.Message ?? "Brand not found.";
                return RedirectToAction("Index");
            }
            return View(BrandMapper.ToViewModel(result.Data!));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BrandViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _brandService.UpdateBrandAsync(id, BrandMapper.ToDto(model));
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message ?? result.Errors?.FirstOrDefault();
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _brandService.DeleteBrandAsync(id);
            if (!result.Success)
                TempData["ErrorMessage"] = result.Message ?? "Failed to delete brand.";
            else
                TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }
    }
}
