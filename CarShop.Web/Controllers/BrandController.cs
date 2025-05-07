using CarShop.Application.DTOs.Brand;
using CarShop.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        // -----------------------------
        // List All Brands
        // -----------------------------
        public async Task<IActionResult> Index()
        {
            var brands = await _brandService.GetAllBrandsAsync();
            return View(brands);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(BrandDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _brandService.CreateBrandAsync(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            return View(brand);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, BrandDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _brandService.UpdateBrandAsync(id, model);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _brandService.DeleteBrandAsync(id);
            return RedirectToAction("Index");
        }
    }
}
