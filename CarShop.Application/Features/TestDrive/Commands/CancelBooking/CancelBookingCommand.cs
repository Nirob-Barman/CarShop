using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.TestDrive.Commands.CancelBooking
{
    public class CancelBookingCommand : IRequest<Result<string>>
    {
        public int BookingId { get; set; }

        public CancelBookingCommand(int bookingId)
        {
            BookingId = bookingId;
        }
    }
}
