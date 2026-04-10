using CarShop.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize]
    public class WishlistController : UserDashboardController
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService, IUserService userService, IUserContextService userContextService)
            : base(userService, userContextService)
        {
            _wishlistService = wishlistService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _wishlistService.GetWishlistAsync();
            return View(result.Data ?? Enumerable.Empty<CarShop.Application.DTOs.Wishlist.WishlistItemDto>());
        }

        [HttpPost]
        public async Task<IActionResult> Add(int carId)
        {
            var result = await _wishlistService.AddToWishlistAsync(carId);
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "Could not add to wishlist.";
            return RedirectToAction("Details", "Car", new { id = carId });
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int carId)
        {
            var result = await _wishlistService.RemoveFromWishlistAsync(carId);
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
