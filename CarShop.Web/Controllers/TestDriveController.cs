using CarShop.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize]
    public class TestDriveController : UserDashboardController
    {
        private readonly ITestDriveService _testDriveService;
        private readonly ICarService _carService;

        public TestDriveController(ITestDriveService testDriveService, ICarService carService, IUserService userService, IUserContextService userContextService)
            : base(userService, userContextService)
        {
            _testDriveService = testDriveService;
            _carService = carService;
        }

        [HttpGet]
        public async Task<IActionResult> Book(int carId)
        {
            var carResult = await _carService.GetCarByIdAsync(carId);
            if (!carResult.Success || carResult.Data == null)
            {
                TempData["ErrorMessage"] = "Car not found.";
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Car = carResult.Data;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(int carId, DateTime bookingDate, string? notes)
        {
            var result = await _testDriveService.BookTestDriveAsync(carId, bookingDate, notes);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("MyBookings");
            }
            TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "Booking failed.";
            var carResult = await _carService.GetCarByIdAsync(carId);
            ViewBag.Car = carResult.Data;
            return View();
        }

        public async Task<IActionResult> MyBookings()
        {
            var result = await _testDriveService.GetUserBookingsAsync();
            return View(result.Data ?? Enumerable.Empty<CarShop.Application.DTOs.TestDrive.TestDriveBookingDto>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int bookingId)
        {
            var result = await _testDriveService.CancelBookingAsync(bookingId);
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "Could not cancel booking.";
            return RedirectToAction("MyBookings");
        }
    }
}
