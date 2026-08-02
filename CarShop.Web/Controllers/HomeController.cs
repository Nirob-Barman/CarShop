using System.Diagnostics;
using CarShop.Application.Features.Brand.Queries.GetAllBrands;
using CarShop.Application.Features.Car.Queries.GetAllCars;
using CarShop.Application.Features.Car.Queries.GetCarsByIds;
using CarShop.Application.Features.Car.Queries.GetRecentCars;
using CarShop.Application.Features.Car.Queries.GetTopRatedCars;
using CarShop.Application.Features.Comment.Queries.GetRecentTestimonials;
using CarShop.Application.Features.Order.Queries.GetCompletedOrdersCount;
using CarShop.Application.Features.PromoCode.Queries.GetActivePromoCodes;
using CarShop.Application.Features.Wishlist.Queries.GetTopWishlistedCars;
using CarShop.Application.Features.Wishlist.Queries.GetWishlist;
using CarShop.Web.Models;
using CarShop.Web.ViewModels.Mappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMediator _mediator;

        public HomeController(
            ILogger<HomeController> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        // ── Tier 1 — server-side (above the fold) ─────────────────
        public async Task<IActionResult> Index()
        {
            // New Arrivals
            var recentResult = await _mediator.Send(new GetRecentCarsQuery(4));
            ViewBag.RecentCars = recentResult.Success
                ? CarMapper.ToViewModels(recentResult.Data!).ToList()
                : new List<CarShop.Web.ViewModels.Car.CarViewModel>();

            // Stats bar
            var carsResult   = await _mediator.Send(new GetAllCarsQuery());
            var brandsResult = await _mediator.Send(new GetAllBrandsQuery());
            var completedOrdersResult = await _mediator.Send(new GetCompletedOrdersCountQuery());
            ViewBag.Brands = BrandMapper.ToViewModels(brandsResult.Data!);
            ViewBag.StatsCarsCount   = carsResult.Success ? carsResult.Data?.Count() ?? 0 : 0;
            ViewBag.StatsBrandsCount = brandsResult.Data?.Count() ?? 0;
            ViewBag.StatsOrdersCount = completedOrdersResult.Success ? completedOrdersResult.Data : 0;

            // Promo banner
            var promoResult = await _mediator.Send(new GetActivePromoCodesQuery());
            ViewBag.ActivePromos = promoResult.Success ? promoResult.Data?.ToList() : null;

            return View();
        }

        // ── Tier 2 — lazy-loaded partials ─────────────────────────

        public async Task<IActionResult> TopRated()
        {
            var result = await _mediator.Send(new GetTopRatedCarsQuery(4));
            if (!result.Success || !result.Data!.Any()) return Content("");
            return PartialView("_TopRatedCars", CarMapper.ToViewModels(result.Data!).ToList());
        }

        public async Task<IActionResult> MostWishlisted()
        {
            var result = await _mediator.Send(new GetTopWishlistedCarsQuery(4));
            if (!result.Success || result.Data == null || !result.Data.Any()) return Content("");
            return PartialView("_TopWishlistedCars", result.Data);
        }

        [Authorize]
        public async Task<IActionResult> MyWishlist()
        {
            if (User.IsInRole("Admin")) return Content("");
            var result = await _mediator.Send(new GetWishlistQuery());
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

            var result = await _mediator.Send(new GetCarsByIdsQuery(ids));
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
            var result = await _mediator.Send(new GetRecentTestimonialsQuery(6));
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
