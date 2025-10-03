using CarShop.Application.DTOs.Brand;
using CarShop.Application.Interfaces;
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

        // -----------------------------
        // List All Brands
        // -----------------------------
        //public async Task<IActionResult> Index()
        //{
        //    var brands = await _brandService.GetAllBrandsAsync();
        //    return View(brands);
        //}

        public async Task<IActionResult> Index()
        {
            var result = await _brandService.GetAllBrandsAsync();
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message ?? "Failed to load brands.";
                return View(new List<BrandDto>());
            }

            return View(result.Data);
        }

        [HttpGet]
        public IActionResult Create() => View();

        //[HttpPost]
        //public async Task<IActionResult> Create(BrandDto model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    try
        //    {
        //        await _brandService.CreateBrandAsync(model);
        //        return RedirectToAction("Index");
        //    }
        //    catch(Exception ex)
        //    {
        //        TempData["ErrorMessage"] = ex.Message;
        //        return View(model);
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> Create(BrandDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _brandService.CreateBrandAsync(model);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message ?? result.Errors?.FirstOrDefault();
                return View(model);
            }

            //TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }

        //[HttpGet]
        //public async Task<IActionResult> Edit(int id)
        //{
        //    var brand = await _brandService.GetBrandByIdAsync(id);
        //    return View(brand);
        //}

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _brandService.GetBrandByIdAsync(id);

            if (!result.Success || result.Data == null)
            {
                TempData["ErrorMessage"] = result.Message ?? "Brand not found.";
                return RedirectToAction("Index");
            }

            return View(result.Data);
        }

        //[HttpPost]
        //public async Task<IActionResult> Edit(int id, BrandDto model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    try
        //    {
        //        await _brandService.UpdateBrandAsync(id, model);
        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = ex.Message;
        //        return View(model);
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> Edit(int id, BrandDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _brandService.UpdateBrandAsync(id, model);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message ?? result.Errors?.FirstOrDefault();
                return View(model);
            }

            //TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }

        //[HttpPost]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    await _brandService.DeleteBrandAsync(id);
        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _brandService.DeleteBrandAsync(id);

            //if (!result.Success)
            //    TempData["ErrorMessage"] = result.Message ?? "Failed to delete brand.";
            //else
            //    TempData["SuccessMessage"] = result.Message;

            return RedirectToAction("Index");
        }
    }
}
