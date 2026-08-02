using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.TestDrive.Commands.BookTestDrive
{
    public class BookTestDriveCommand : IRequest<Result<string>>
    {
        public int CarId { get; set; }
        public DateTime BookingDate { get; set; }
        public string? Notes { get; set; }

        public BookTestDriveCommand(int carId, DateTime bookingDate, string? notes)
        {
            CarId = carId;
            BookingDate = bookingDate;
            Notes = notes;
        }
    }
}
