using CarShop.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "User")]
    public class ReviewController : UserDashboardController
    {
        private readonly ICommentService _commentService;

        public ReviewController(ICommentService commentService, IUserService userService, IUserContextService userContextService)
            : base(userService, userContextService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> MyReviews()
        {
            var result = await _commentService.GetUserReviewsAsync();
            return View(result.Data ?? Enumerable.Empty<CarShop.Application.DTOs.Comment.CommentDto>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int carId, string content, int rating)
        {
            var result = await _commentService.AddReviewAsync(carId, content, rating);
            if (!result.Success)
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "Could not submit review.";
            else
                TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Details", "Car", new { id = carId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReview(int commentId, int carId, string content, int rating)
        {
            var result = await _commentService.EditReviewAsync(commentId, content, rating);
            if (!result.Success)
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "Could not update review.";
            else
                TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Details", "Car", new { id = carId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int commentId, int carId)
        {
            var result = await _commentService.DeleteReviewAsync(commentId);
            if (!result.Success)
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "Could not delete review.";
            else
                TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Details", "Car", new { id = carId });
        }
    }
}
