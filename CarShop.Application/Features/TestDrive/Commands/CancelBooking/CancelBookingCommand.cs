using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.TestDrive.Commands.CancelBooking
{
    public record CancelBookingCommand(int BookingId)
        : IRequest<Result<string>>;
}
