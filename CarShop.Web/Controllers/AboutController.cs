using CarShop.Application.Features.Brand.Queries.GetAllBrands;
using CarShop.Application.Features.Car.Queries.GetAllCars;
using CarShop.Application.Features.Comment.Queries.GetAllReviews;
using CarShop.Application.Features.Order.Queries.GetCompletedOrdersCount;
using CarShop.Web.ViewModels.Home;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    public class AboutController : Controller
    {
        private readonly IMediator _mediator;

        public AboutController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var carsResult    = await _mediator.Send(new GetAllCarsQuery());
            var brandsResult  = await _mediator.Send(new GetAllBrandsQuery());
            var reviewsResult = await _mediator.Send(new GetAllReviewsQuery());
            var happyCustomersResult = await _mediator.Send(new GetCompletedOrdersCountQuery());

            var model = new AboutViewModel
            {
                CarsCount      = carsResult.Success    ? carsResult.Data?.Count()    ?? 0 : 0,
                BrandsCount    = brandsResult.Success  ? brandsResult.Data?.Count()  ?? 0 : 0,
                ReviewsCount   = reviewsResult.Success ? reviewsResult.Data?.Count() ?? 0 : 0,
                HappyCustomers = happyCustomersResult.Success ? happyCustomersResult.Data : 0
            };

            ViewData["Title"]           = "About Us";
            ViewData["MetaDescription"] = "Learn about CarShop — our mission, values, and the team behind the platform.";
            return View(model);
        }
    }
}
