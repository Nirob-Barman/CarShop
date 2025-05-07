using CarShop.Application.DTOs.Car;
using CarShop.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
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
        public async Task<IActionResult> Create(CarDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Brands = await _brandService.GetAllBrandsAsync();
                return View(model);
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
