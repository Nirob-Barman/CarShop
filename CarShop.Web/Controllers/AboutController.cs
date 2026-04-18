using CarShop.Application.Interfaces;
using CarShop.Web.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    public class AboutController : Controller
    {
        private readonly ICarService _carService;
        private readonly IBrandService _brandService;
        private readonly ICommentService _commentService;
        private readonly IOrderService _orderService;

        public AboutController(
            ICarService carService,
            IBrandService brandService,
            ICommentService commentService,
            IOrderService orderService)
        {
            _carService = carService;
            _brandService = brandService;
            _commentService = commentService;
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var carsResult    = await _carService.GetAllCarsAsync();
            var brandsResult  = await _brandService.GetAllBrandsAsync();
            var reviewsResult = await _commentService.GetAllReviewsAsync();
            var happyCustomers = await _orderService.GetCompletedOrdersCountAsync();

            var model = new AboutViewModel
            {
                CarsCount      = carsResult.Success    ? carsResult.Data?.Count()    ?? 0 : 0,
                BrandsCount    = brandsResult.Success  ? brandsResult.Data?.Count()  ?? 0 : 0,
                ReviewsCount   = reviewsResult.Success ? reviewsResult.Data?.Count() ?? 0 : 0,
                HappyCustomers = happyCustomers
            };

            ViewData["Title"]           = "About Us";
            ViewData["MetaDescription"] = "Learn about CarShop — our mission, values, and the team behind the platform.";
            return View(model);
        }
    }
}
