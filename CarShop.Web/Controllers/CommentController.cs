using CarShop.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        //[Authorize]
        //[HttpPost]
        //public async Task<IActionResult> AddComment(int carId, string content)
        //{
        //    var userName = User.Identity?.Name;
        //    await _commentService.AddCommentAsync(carId, userName!, content);
        //    return RedirectToAction("Details", "Home", new { id = carId });
        //}

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment(int carId, string content)
        {
            var userName = User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(userName))
            {
                //TempData["ErrorMessage"] = "User identity not found.";
                return RedirectToAction("Details", "Home", new { id = carId });
            }

            var result = await _commentService.AddCommentAsync(carId, userName, content);

            //if (!result.Success)
            //{
            //    TempData["ErrorMessage"] = result.Message;
            //}

            return RedirectToAction("Details", "Home", new { id = carId });
        }
    }
}
