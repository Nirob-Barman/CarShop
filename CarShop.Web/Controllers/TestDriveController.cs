using CarShop.Application.Features.Car.Queries.GetCarById;
using CarShop.Application.Features.TestDrive.Commands.BookTestDrive;
using CarShop.Application.Features.TestDrive.Commands.CancelBooking;
using CarShop.Application.Features.TestDrive.Queries.GetUserBookings;
using CarShop.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize]
    public class TestDriveController : UserDashboardController
    {
        private readonly IMediator _mediator;

        public TestDriveController(IMediator mediator, IUserContextService userContextService)
            : base(mediator, userContextService)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Book(int carId)
        {
            var carResult = await _mediator.Send(new GetCarByIdQuery(carId));
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
            var result = await _mediator.Send(new BookTestDriveCommand(carId, bookingDate, notes));
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("MyBookings");
            }
            TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "Booking failed.";
            var carResult = await _mediator.Send(new GetCarByIdQuery(carId));
            ViewBag.Car = carResult.Data;
            return View();
        }

        public async Task<IActionResult> MyBookings()
        {
            var result = await _mediator.Send(new GetUserBookingsQuery());
            return View(result.Data ?? Enumerable.Empty<CarShop.Application.DTOs.TestDrive.TestDriveBookingDto>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int bookingId)
        {
            var result = await _mediator.Send(new CancelBookingCommand(bookingId));
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "Could not cancel booking.";
            return RedirectToAction("MyBookings");
        }
    }
}
