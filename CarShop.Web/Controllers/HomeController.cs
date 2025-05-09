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

        public async Task<IActionResult> Index(string? brandName)
        {
            int? brandId = null;

            if (!string.IsNullOrWhiteSpace(brandName))
            {
                var brand = await _brandService.GetBrandByNameAsync(brandName);
                if (brand != null)
                {
                    brandId = brand.Id;
                    ViewBag.SelectedBrand = brand.Name;
                }
            }

            var cars = brandId.HasValue
                ? await _carService.GetCarsByBrandIdAsync(brandId.Value)
                : await _carService.GetAllCarsAsync();

            ViewBag.Brands = await _brandService.GetAllBrandsAsync();
            return View(cars);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null) return NotFound();
            ViewBag.Comments = await _commentService.GetCommentsByCarIdAsync(id);
            return View(car);
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
