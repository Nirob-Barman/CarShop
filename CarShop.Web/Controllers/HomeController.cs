using System.Diagnostics;
using CarShop.Application.Interfaces;
using CarShop.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICarService _carService;
        private readonly IBrandService _brandService;
        private readonly ICommentService _commentService;

        public HomeController(ILogger<HomeController> logger, ICarService carService, IBrandService brandService, ICommentService commentService)
        {
            _logger = logger;
            _carService = carService;
            _brandService = brandService;
            _commentService = commentService;
        }

        //public async Task<IActionResult> Index(string? brandName)
        //{
        //    int page = 1;
        //    int pageSize = 4;
        //    int? brandId = null;

        //    if (!string.IsNullOrWhiteSpace(brandName))
        //    {
        //        var brand = await _brandService.GetBrandByNameAsync(brandName);
        //        if (brand != null)
        //        {
        //            brandId = brand.Id;
        //            ViewBag.SelectedBrand = brand.Name;
        //        }
        //    }

        //    var cars = brandId.HasValue
        //        ? await _carService.GetCarsByBrandIdAsync(brandId.Value)
        //        : await _carService.GetAllCarsAsync();

        //    var totalCars = cars.Count();
        //    ViewBag.TotalPages = (int)Math.Ceiling((double)totalCars / pageSize);
        //    var paginatedCars = cars.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        //    ViewBag.ShowAllButton = totalCars > pageSize;

        //    ViewBag.Brands = await _brandService.GetAllBrandsAsync();
        //    return View(paginatedCars);
        //}

        public async Task<IActionResult> Index(string? brandName)
        {
            int page = 1;
            int pageSize = 4;
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
                TempData["ErrorMessage"] = carResult.Message ?? "Failed to load cars.";
                return View(new List<object>());
            }

            var cars = carResult.Data!;
            int totalCars = cars.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCars / pageSize);
            var paginatedCars = cars.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.ShowAllButton = totalCars > pageSize;

            var brandsResult = await _brandService.GetAllBrandsAsync();
            ViewBag.Brands = brandsResult.Data;

            return View(paginatedCars);
        }

        //[HttpGet]
        //public async Task<IActionResult> Details(int id)
        //{
        //    var car = await _carService.GetCarByIdAsync(id);
        //    if (car == null) return NotFound();
        //    ViewBag.Comments = await _commentService.GetCommentsByCarIdAsync(id);
        //    return View(car);
        //}

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var carResult = await _carService.GetCarByIdAsync(id);
            if (!carResult.Success || carResult.Data == null)
            {
                TempData["ErrorMessage"] = carResult.Message ?? "Car not found.";
                return RedirectToAction("Index");
            }

            var commentResult = await _commentService.GetCommentsByCarIdAsync(id);
            if (!commentResult.Success)
            {
                TempData["ErrorMessage"] = commentResult.Message ?? "Failed to load comments.";
                ViewBag.Comments = new List<object>();
            }
            else
            {
                ViewBag.Comments = commentResult.Data;
            }

            return View(carResult.Data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
