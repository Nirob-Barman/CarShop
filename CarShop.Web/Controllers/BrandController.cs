using CarShop.Application.DTOs.Brand;
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

            //var vm = result.Data!.Select((b => new BrandViewModel { Id = b.Id, Name = b.Name }));
            var vm = BrandMapper.ToViewModels(result.Data!);

            return View(vm);
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
        public async Task<IActionResult> Create(BrandViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            //var dto = new BrandDto
            //{
            //    Name = model.Name?.Trim()
            //};
            var dto = BrandMapper.ToDto(model);

            var result = await _brandService.CreateBrandAsync(dto);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message ?? result.Errors?.FirstOrDefault();
                return View(model);
            }

            //TempData["SuccessMessage"] = result.Message;
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

            var vm = BrandMapper.ToViewModel(result.Data!);

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, BrandViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = BrandMapper.ToDto(model);

            var result = await _brandService.UpdateBrandAsync(id, dto);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message ?? result.Errors?.FirstOrDefault();
                return View(model);
            }

            //TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }


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
