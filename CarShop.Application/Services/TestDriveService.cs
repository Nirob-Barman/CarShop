using CarShop.Application.DTOs.TestDrive;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using System.Text.Json;

namespace CarShop.Application.Services
{
    public class TestDriveService : ITestDriveService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public TestDriveService(IUnitOfWork unitOfWork, IAuditLogService auditLogService, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> BookTestDriveAsync(int carId, DateTime bookingDate, string? notes)
        {
            var userId = _userContextService.UserId!;

            if (bookingDate <= DateTime.UtcNow)
                return Result<string>.Fail("Booking date must be in the future.");

            var car = await _unitOfWork.Repository<Car>().GetByIdAsync(carId);
            if (car == null)
                return Result<string>.Fail("Car not found.");

            var recentBooking = await _unitOfWork.Repository<TestDriveBooking>().AnyAsync(
                b => b.UserId == userId && b.CarId == carId &&
                     b.Status != "Cancelled" &&
                     b.BookingDate >= DateTime.UtcNow.AddDays(-7));

            if (recentBooking)
                return Result<string>.Fail("You already have a booking for this car within the last 7 days.");

            var booking = new TestDriveBooking
            {
                UserId = userId,
                CarId = carId,
                BookingDate = bookingDate,
                Notes = notes,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<TestDriveBooking>().AddAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("TestDrive", "Book",
                _userContextService.UserId, _userContextService.Email,
                $"Booked test drive for car '{car.Title}' on {bookingDate:yyyy-MM-dd}",
                entityId: booking.Id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                newValues: JsonSerializer.Serialize(new
                {
                    booking.CarId, CarTitle = car.Title,
                    BookingDate = bookingDate.ToString("yyyy-MM-dd HH:mm"),
                    booking.Notes, booking.Status
                }));

            return Result<string>.Ok(null, "Test drive booked successfully. We'll confirm your booking shortly.");
        }

        public async Task<Result<IEnumerable<TestDriveBookingDto>>> GetUserBookingsAsync()
        {
            var userId = _userContextService.UserId!;
            var bookings = await _unitOfWork.Repository<TestDriveBooking>().GetAllWithIncludesAsync(
                predicate: b => b.UserId == userId,
                selector: b => b,
                b => b.Car!
            );

            var dtos = bookings.OrderByDescending(b => b.CreatedAt).Select(b => new TestDriveBookingDto
            {
                Id = b.Id,
                UserId = b.UserId,
                CarId = b.CarId,
                CarTitle = b.Car?.Title,
                BookingDate = b.BookingDate,
                Notes = b.Notes,
                Status = b.Status,
                CreatedAt = b.CreatedAt
            });

            return Result<IEnumerable<TestDriveBookingDto>>.Ok(dtos);
        }

        public async Task<Result<IEnumerable<TestDriveBookingDto>>> GetAllBookingsAsync(string? status = null)
        {
            var bookings = await _unitOfWork.Repository<TestDriveBooking>().GetAllWithIncludesAsync(
                predicate: b => status == null || b.Status == status,
                selector: b => b,
                b => b.Car!
            );

            var dtos = bookings.OrderByDescending(b => b.CreatedAt).Select(b => new TestDriveBookingDto
            {
                Id = b.Id,
                UserId = b.UserId,
                CarId = b.CarId,
                CarTitle = b.Car?.Title,
                BookingDate = b.BookingDate,
                Notes = b.Notes,
                Status = b.Status,
                CreatedAt = b.CreatedAt
            });

            return Result<IEnumerable<TestDriveBookingDto>>.Ok(dtos);
        }

        public async Task<Result<string>> UpdateStatusAsync(int bookingId, string status)
        {
            var booking = await _unitOfWork.Repository<TestDriveBooking>().GetByIdAsync(bookingId);
            if (booking == null)
                return Result<string>.Fail("Booking not found.");

            var oldStatus = booking.Status;
            booking.Status = status;
            _unitOfWork.Repository<TestDriveBooking>().Update(booking);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("TestDrive", "StatusUpdate",
                _userContextService.UserId, _userContextService.Email,
                $"Status changed from '{oldStatus}' to '{status}'",
                entityId: bookingId,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                oldValues: JsonSerializer.Serialize(new { Status = oldStatus }),
                newValues: JsonSerializer.Serialize(new { Status = status }));

            return Result<string>.Ok(null, $"Booking status updated to {status}.");
        }

        public async Task<Result<string>> CancelBookingAsync(int bookingId)
        {
            var userId = _userContextService.UserId!;
            var booking = await _unitOfWork.Repository<TestDriveBooking>().FirstOrDefaultAsync(
                b => b.Id == bookingId && b.UserId == userId);

            if (booking == null)
                return Result<string>.Fail("Booking not found or you are not authorized to cancel it.");

            if (booking.Status == "Cancelled")
                return Result<string>.Fail("Booking is already cancelled.");

            var oldStatus = booking.Status;
            booking.Status = "Cancelled";
            _unitOfWork.Repository<TestDriveBooking>().Update(booking);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("TestDrive", "Cancel",
                _userContextService.UserId, _userContextService.Email,
                entityId: bookingId,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                oldValues: JsonSerializer.Serialize(new { Status = oldStatus }),
                newValues: JsonSerializer.Serialize(new { Status = "Cancelled" }));

            return Result<string>.Ok(null, "Test drive booking cancelled.");
        }
    }
}
