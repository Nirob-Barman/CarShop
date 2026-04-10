using CarShop.Application.DTOs.TestDrive;
using CarShop.Application.Wrappers;

namespace CarShop.Application.Interfaces
{
    public interface ITestDriveService
    {
        Task<Result<string>> BookTestDriveAsync(int carId, DateTime bookingDate, string? notes);
        Task<Result<IEnumerable<TestDriveBookingDto>>> GetUserBookingsAsync();
        Task<Result<IEnumerable<TestDriveBookingDto>>> GetAllBookingsAsync(string? status = null);
        Task<Result<string>> UpdateStatusAsync(int bookingId, string status);
        Task<Result<string>> CancelBookingAsync(int bookingId);
    }
}
