using System.Diagnostics;
using CarShop.Application.Interfaces;
using CarShop.Web.Models;
using CarShop.Web.ViewModels.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICarService _carService;
        private readonly IBrandService _brandService;
        private readonly ICommentService _commentService;
        private readonly IWishlistService _wishlistService;
        private readonly IPromoCodeService _promoCodeService;
        private readonly IOrderService _orderService;

        public HomeController(
            ILogger<HomeController> logger,
            ICarService carService,
            IBrandService brandService,
            ICommentService commentService,
            IWishlistService wishlistService,
            IPromoCodeService promoCodeService,
            IOrderService orderService)
        {
            _logger = logger;
            _carService = carService;
            _brandService = brandService;
            _commentService = commentService;
            _wishlistService = wishlistService;
            _promoCodeService = promoCodeService;
            _orderService = orderService;
        }

        // ── Tier 1 — server-side (above the fold) ─────────────────
        public async Task<IActionResult> Index(string? brandName)
        {
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
                return View(new List<CarShop.Web.ViewModels.Car.CarViewModel>());
            }

            var cars = CarMapper.ToViewModels(carResult.Data!);
            int totalCars = cars.Count();
            var paginatedCars = cars.Take(pageSize).ToList();
            ViewBag.ShowAllButton = totalCars > pageSize;

            var brandsResult = await _brandService.GetAllBrandsAsync();
            ViewBag.Brands = BrandMapper.ToViewModels(brandsResult.Data!);

            // New Arrivals
            var recentResult = await _carService.GetRecentCarsAsync(4);
            ViewBag.RecentCars = recentResult.Success
                ? CarMapper.ToViewModels(recentResult.Data!).ToList()
                : new List<CarShop.Web.ViewModels.Car.CarViewModel>();

            // Stats bar
            var completedOrders = await _orderService.GetCompletedOrdersCountAsync();
            ViewBag.StatsCarsCount   = totalCars;
            ViewBag.StatsBrandsCount = brandsResult.Data?.Count() ?? 0;
            ViewBag.StatsOrdersCount = completedOrders;

            // Promo banner
            var promoResult = await _promoCodeService.GetAllActiveCodesAsync();
            ViewBag.ActivePromos = promoResult.Success ? promoResult.Data?.ToList() : null;

            return View(paginatedCars);
        }

        // ── Tier 2 — lazy-loaded partials ─────────────────────────

        public async Task<IActionResult> TopRated()
        {
            var result = await _carService.GetTopRatedCarsAsync(4);
            if (!result.Success || !result.Data!.Any()) return Content("");
            return PartialView("_TopRatedCars", CarMapper.ToViewModels(result.Data!).ToList());
        }

        public async Task<IActionResult> MostWishlisted()
        {
            var result = await _wishlistService.GetTopWishlistedCarsAsync(4);
            if (!result.Success || result.Data == null || !result.Data.Any()) return Content("");
            return PartialView("_TopWishlistedCars", result.Data);
        }

        [Authorize]
        public async Task<IActionResult> MyWishlist()
        {
            if (User.IsInRole("Admin")) return Content("");
            var result = await _wishlistService.GetWishlistAsync();
            if (!result.Success || result.Data == null || !result.Data.Any()) return Content("");
            return PartialView("_WishlistSection", result.Data);
        }

        public async Task<IActionResult> RecentlyViewed()
        {
            var cookie = Request.Cookies["RecentlyViewed"] ?? "";
            var ids = cookie
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Where(x => int.TryParse(x, out _))
                .Select(int.Parse)
                .Take(8)
                .ToList();

            if (!ids.Any()) return Content("");

            var result = await _carService.GetCarsByIdsAsync(ids);
            if (!result.Success || result.Data == null) return Content("");

            var ordered = ids
                .Select(id => result.Data.FirstOrDefault(c => c.Id == id))
                .Where(c => c != null)
                .ToList();

            if (!ordered.Any()) return Content("");
            return PartialView("_RecentlyViewed", CarMapper.ToViewModels(ordered!).ToList());
        }

        public async Task<IActionResult> Testimonials()
        {
            var result = await _commentService.GetRecentTestimonialsAsync(6);
            if (!result.Success || result.Data == null || !result.Data.Any()) return Content("");
            return PartialView("_Testimonials", result.Data);
        }

        // ── Misc ───────────────────────────────────────────────────
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode == 404)
                return View("NotFound");

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
