using CarShop.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TestDrivesController : Controller
    {
        private readonly ITestDriveService _testDriveService;

        public TestDrivesController(ITestDriveService testDriveService)
        {
            _testDriveService = testDriveService;
        }

        public async Task<IActionResult> Index(string? status)
        {
            var result = await _testDriveService.GetAllBookingsAsync(status);
            ViewBag.Status = status;
            return View(result.Data ?? Enumerable.Empty<CarShop.Application.DTOs.TestDrive.TestDriveBookingDto>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int bookingId, string status)
        {
            var result = await _testDriveService.UpdateStatusAsync(bookingId, status);
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "Could not update status.";
            return RedirectToAction("Index");
        }
    }
}
