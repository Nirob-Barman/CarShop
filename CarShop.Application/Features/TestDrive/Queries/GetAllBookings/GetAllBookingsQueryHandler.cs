using CarShop.Application.DTOs.TestDrive;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using MediatR;

namespace CarShop.Application.Features.TestDrive.Queries.GetAllBookings
{
    public class GetAllBookingsQueryHandler : IRequestHandler<GetAllBookingsQuery, Result<IEnumerable<TestDriveBookingDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllBookingsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<TestDriveBookingDto>>> Handle(GetAllBookingsQuery request, CancellationToken cancellationToken)
        {
            var bookings = await _unitOfWork.Repository<TestDriveBooking>().GetAllWithIncludesAsync(
                predicate: b => request.Status == null || b.Status == request.Status,
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
    }
}
