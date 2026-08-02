using CarShop.Application.Features.Comment.Commands.AddReview;
using CarShop.Application.Features.Comment.Commands.DeleteReview;
using CarShop.Application.Features.Comment.Commands.EditReview;
using CarShop.Application.Features.Comment.Queries.GetUserReviews;
using CarShop.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "User")]
    public class ReviewController : UserDashboardController
    {
        private readonly IMediator _mediator;

        public ReviewController(IMediator mediator, IUserContextService userContextService)
            : base(mediator, userContextService)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> MyReviews()
        {
            var result = await _mediator.Send(new GetUserReviewsQuery());
            return View(result.Data ?? Enumerable.Empty<CarShop.Application.DTOs.Comment.CommentDto>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int carId, string content, int rating)
        {
            var result = await _mediator.Send(new AddReviewCommand(carId, content, rating));
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
            var result = await _mediator.Send(new EditReviewCommand(commentId, content, rating));
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
            var result = await _mediator.Send(new DeleteReviewCommand(commentId));
            if (!result.Success)
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "Could not delete review.";
            else
                TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Details", "Car", new { id = carId });
        }
    }
}
