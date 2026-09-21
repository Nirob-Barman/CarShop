using CarShop.Application.Interfaces;
using CarShop.Application.DTOs.TestDrive;
using CarShop.Application.Wrappers;
using CarShop.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.TestDrive.Queries.GetAllBookings
{
    public class GetAllBookingsQueryHandler : IRequestHandler<GetAllBookingsQuery, Result<IEnumerable<TestDriveBookingDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllBookingsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IEnumerable<TestDriveBookingDto>>> Handle(GetAllBookingsQuery request, CancellationToken cancellationToken)
        {
            TestDriveStatus? statusFilter = Enum.TryParse<TestDriveStatus>(request.Status, ignoreCase: true, out var parsed) ? parsed : null;

            var bookings = await _context.TestDriveBookings
                .Include(b => b.Car)
                .Where(b => statusFilter == null || b.Status == statusFilter)
                .ToListAsync(cancellationToken);

            var dtos = bookings.OrderByDescending(b => b.CreatedAt).Select(b => new TestDriveBookingDto
            {
                Id = b.Id,
                UserId = b.UserId,
                CarId = b.CarId,
                CarTitle = b.Car?.Title,
                BookingDate = b.BookingDate,
                Notes = b.Notes,
                Status = b.Status.ToString(),
                CreatedAt = b.CreatedAt
            });

            return Result<IEnumerable<TestDriveBookingDto>>.Ok(dtos);
        }
    }
}
