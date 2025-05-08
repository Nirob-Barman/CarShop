using CarShop.Application.DTOs.Car;
using CarShop.Application.Interfaces;
using CarShop.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    public class CarController : Controller
    {
        private readonly ICarService _carService;
        private readonly IBrandService _brandService;
        private readonly ImageService _imageService;
        public CarController(ICarService carService, IBrandService brandService, ImageService imageService)
        {
            _carService = carService;
            _brandService = brandService;
            _imageService = imageService;
        }

        public async Task<IActionResult> Index()
        {
            var cars = await _carService.GetAllCarsAsync();
            return View(cars);
        }
        
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Brands = await _brandService.GetAllBrandsAsync();
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(CarDto model, IFormFile image)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Brands = await _brandService.GetAllBrandsAsync();
                return View(model);
            }

            if (image != null && image.Length > 0)
            {
                var sanitizedTitle = model.Title!.Replace(" ", "_").ToLowerInvariant(); //String.ToLower() uses the default culture while String.ToLowerInvariant() uses the invariant culture
                var fileName = $"car_{sanitizedTitle}_{DateTime.UtcNow.Ticks}{Path.GetExtension(image.FileName)}";

                using var stream = image.OpenReadStream();
                var imageUrl = await _imageService.UploadImageAsync(stream, fileName, image.ContentType);
                model.ImageUrl = imageUrl;
            }

            await _carService.CreateCarAsync(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            ViewBag.Brands = await _brandService.GetAllBrandsAsync();
            return View(car);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CarDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Brands = await _brandService.GetAllBrandsAsync();
                return View(model);
            }

            await _carService.UpdateCarAsync(id, model);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _carService.DeleteCarAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            return View(car);
        }
    }
}
