using CarShop.Application.DTOs.TestDrive;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.TestDrive.Queries.GetUserBookings
{
    public class GetUserBookingsQuery : IRequest<Result<IEnumerable<TestDriveBookingDto>>>
    {
    }
}
