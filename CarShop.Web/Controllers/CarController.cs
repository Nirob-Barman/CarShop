using CarShop.Application.DTOs.Car;
using CarShop.Application.Interfaces;
using CarShop.Web.ViewModels.Car;
using CarShop.Web.ViewModels.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CarController : Controller
    {
        private readonly ICarService _carService;
        private readonly IBrandService _brandService;
        public CarController(ICarService carService, IBrandService brandService)
        {
            _carService = carService;
            _brandService = brandService;
        }
        
        public async Task<IActionResult> Index()
        {
            var result = await _carService.GetAllCarsAsync();
            if (!result.Success)
            {
                //TempData["ErrorMessage"] = result.Message ?? "Failed to load cars.";
                return View(new List<CarViewModel>());
            }
            var vm = CarMapper.ToViewModels(result.Data!);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var brandResult = await _brandService.GetAllBrandsAsync();
            ViewBag.Brands = BrandMapper.ToViewModels(brandResult.Data!);
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(CarViewModel model, IFormFile image)
        {
            var brandResult = await _brandService.GetAllBrandsAsync();            
            ViewBag.Brands = BrandMapper.ToViewModels(brandResult.Data!);

            if (!ModelState.IsValid)
                return View(model);

            if (image != null && image.Length > 0)
            {
                var fileSafeTitle = model.Title!.ToLowerInvariant().Replace(" ", "_");
                var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "car");

                if (!Directory.Exists(uploadsRoot))
                    Directory.CreateDirectory(uploadsRoot);

                string fileName = $"{fileSafeTitle}{Path.GetExtension(image.FileName)}";
                var filePath = Path.Combine(uploadsRoot, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                model.ImageUrl = Path.Combine("/uploads", "car", fileName).Replace("\\", "/");
            }

            var dto = CarMapper.ToDto(model);
            var result = await _carService.CreateCarAsync(dto);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return View(model);
            }

            //TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var carResult = await _carService.GetCarByIdAsync(id);
            var brandResult = await _brandService.GetAllBrandsAsync();

            //if (!carResult.Success || carResult.Data == null)
            //{
            //    TempData["ErrorMessage"] = carResult.Message ?? "Car not found.";
            //    return RedirectToAction("Index");
            //}

            ViewBag.Brands = BrandMapper.ToViewModels(brandResult.Data!);
            var vm = CarMapper.ToViewModel(carResult.Data!);
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(int id, CarViewModel model, IFormFile? image)
        {
            var brandResult = await _brandService.GetAllBrandsAsync();
            ViewBag.Brands = brandResult.Data;

            if (!ModelState.IsValid)
                return View(model);

            var existingCarResult = await _carService.GetCarByIdAsync(id);
            if (!existingCarResult.Success || existingCarResult.Data == null)
            {
                TempData["ErrorMessage"] = existingCarResult.Message ?? "Car not found.";
                return RedirectToAction("Index");
            }

            var existingCar = existingCarResult.Data;

            if (image != null && image.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingCar.ImageUrl))
                {
                    var previousPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingCar.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(previousPath))
                        System.IO.File.Delete(previousPath);
                }

                var fileSafeTitle = model.Title!.ToLowerInvariant().Replace(" ", "_");
                var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "car");
                if (!Directory.Exists(uploadsRoot))
                    Directory.CreateDirectory(uploadsRoot);

                string fileName = $"{fileSafeTitle}{Path.GetExtension(image.FileName)}";
                var filePath = Path.Combine(uploadsRoot, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                model.ImageUrl = Path.Combine("/uploads", "car", fileName).Replace("\\", "/");
            }
            else
            {
                model.ImageUrl = existingCar.ImageUrl;
            }

            var dto = CarMapper.ToDto(model);
            var result = await _carService.UpdateCarAsync(id, dto);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return View(model);
            }

            //TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var carResult = await _carService.GetCarByIdAsync(id);
            if (!carResult.Success || carResult.Data == null)
            {
                TempData["ErrorMessage"] = carResult.Message ?? "Car not found.";
                return RedirectToAction("Index");
            }

            var car = carResult.Data;

            if (!string.IsNullOrEmpty(car.ImageUrl))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", car.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }

            var deleteResult = await _carService.DeleteCarAsync(id);
            //if (!deleteResult.Success)
            //    TempData["ErrorMessage"] = deleteResult.Message;
            //else
            //    TempData["SuccessMessage"] = deleteResult.Message;

            return RedirectToAction("Index");
        }

        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<IActionResult> Details(int id)
        //{
        //    var car = await _carService.GetCarByIdAsync(id);
        //    return View(car);
        //}

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _carService.GetCarByIdAsync(id);
            //if (!result.Success || result.Data == null)
            //{
            //    TempData["ErrorMessage"] = result.Message ?? "Car not found.";
            //    return RedirectToAction("AllCars");
            //}
            var vm = CarMapper.ToViewModel(result.Data!);
            return View(vm);
        }



        [AllowAnonymous]
        public async Task<IActionResult> AllCars(string? brandName, int page = 1)
        {
            int pageSize = 10;
            int? brandId = null;

            if (!string.IsNullOrWhiteSpace(brandName))
            {
                var brandResult = await _brandService.GetBrandByNameAsync(brandName);
                if (brandResult.Success && brandResult.Data != null)
                {
                    brandId = brandResult.Data.Id;
                    ViewBag.SelectedBrand = brandResult.Data.Name;
                }
            }

            var carResult = brandId.HasValue
                ? await _carService.GetCarsByBrandIdAsync(brandId.Value)
                : await _carService.GetAllCarsAsync();

            if (!carResult.Success)
            {
                //TempData["ErrorMessage"] = carResult.Message;
                return View(new List<CarDto>());
            }

            var allCars = CarMapper.ToViewModels(carResult.Data!);
            int totalCars = allCars.Count();
            var paginatedCars = allCars
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCars / pageSize);
            ViewBag.CurrentPage = page;

            var brandList = await _brandService.GetAllBrandsAsync();
            ViewBag.Brands = BrandMapper.ToViewModels(brandList.Data!);

            return View(paginatedCars);
        }
    }
}
