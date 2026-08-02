using CarShop.Application.DTOs.TestDrive;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.TestDrive.Queries.GetAllBookings
{
    public class GetAllBookingsQuery : IRequest<Result<IEnumerable<TestDriveBookingDto>>>
    {
        public string? Status { get; set; }

        public GetAllBookingsQuery(string? status = null)
        {
            Status = status;
        }
    }
}
