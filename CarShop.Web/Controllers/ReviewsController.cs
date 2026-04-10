using CarShop.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReviewsController : Controller
    {
        private readonly ICommentService _commentService;

        public ReviewsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _commentService.GetAllReviewsAsync();
            return View(result.Data ?? Enumerable.Empty<CarShop.Application.DTOs.Comment.CommentDto>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int commentId)
        {
            var result = await _commentService.DeleteReviewAsync(commentId);
            if (!result.Success)
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "Could not delete review.";
            else
                TempData["SuccessMessage"] = "Review deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
