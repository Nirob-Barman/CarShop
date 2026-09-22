using CarShop.Application.DTOs.TestDrive;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.TestDrive.Queries.GetAllBookings
{
    public record GetAllBookingsQuery(string? Status = null)
        : IRequest<Result<IEnumerable<TestDriveBookingDto>>>;
}
