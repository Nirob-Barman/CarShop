using CarShop.Application.Features.Wishlist.Commands.AddToWishlist;
using CarShop.Application.Features.Wishlist.Commands.RemoveFromWishlist;
using CarShop.Application.Features.Wishlist.Queries.GetWishlist;
using CarShop.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize]
    public class WishlistController : UserDashboardController
    {
        private readonly IMediator _mediator;

        public WishlistController(IMediator mediator, IUserContextService userContextService)
            : base(mediator, userContextService)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _mediator.Send(new GetWishlistQuery());
            return View(result.Data ?? Enumerable.Empty<CarShop.Application.DTOs.Wishlist.WishlistItemDto>());
        }

        [HttpPost]
        public async Task<IActionResult> Add(int carId)
        {
            var result = await _mediator.Send(new AddToWishlistCommand(carId));
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "Could not add to wishlist.";
            return RedirectToAction("Details", "Car", new { id = carId });
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int carId)
        {
            var result = await _mediator.Send(new RemoveFromWishlistCommand(carId));
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "Could not remove from wishlist.";

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
                return Redirect(referer);
            return RedirectToAction("Index");
        }
    }
}
