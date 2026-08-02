using CarShop.Application.Features.TestDrive.Commands.UpdateStatus;
using CarShop.Application.Features.TestDrive.Queries.GetAllBookings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TestDrivesController : Controller
    {
        private readonly IMediator _mediator;

        public TestDrivesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(string? status)
        {
            var result = await _mediator.Send(new GetAllBookingsQuery(status));
            ViewBag.Status = status;
            return View(result.Data ?? Enumerable.Empty<CarShop.Application.DTOs.TestDrive.TestDriveBookingDto>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int bookingId, string status)
        {
            var result = await _mediator.Send(new UpdateStatusCommand(bookingId, status));
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "Could not update status.";
            return RedirectToAction("Index");
        }
    }
}
