using CarShop.Application.Features.Comment.Commands.DeleteReview;
using CarShop.Application.Features.Comment.Queries.GetAllReviews;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReviewsController : Controller
    {
        private readonly IMediator _mediator;

        public ReviewsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _mediator.Send(new GetAllReviewsQuery());
            return View(result.Data ?? Enumerable.Empty<CarShop.Application.DTOs.Comment.CommentDto>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int commentId)
        {
            var result = await _mediator.Send(new DeleteReviewCommand(commentId));
            if (!result.Success)
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "Could not delete review.";
            else
                TempData["SuccessMessage"] = "Review deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
