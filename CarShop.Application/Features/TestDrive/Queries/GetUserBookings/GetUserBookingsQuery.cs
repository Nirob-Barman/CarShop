using CarShop.Application.DTOs.TestDrive;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.TestDrive.Queries.GetUserBookings
{
    public record GetUserBookingsQuery
        : IRequest<Result<IEnumerable<TestDriveBookingDto>>>;
}
