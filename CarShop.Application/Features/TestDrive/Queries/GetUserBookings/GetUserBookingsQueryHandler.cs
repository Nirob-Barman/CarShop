using CarShop.Application.DTOs.TestDrive;
using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.TestDrive.Queries.GetUserBookings
{
    public class GetUserBookingsQueryHandler : IRequestHandler<GetUserBookingsQuery, Result<IEnumerable<TestDriveBookingDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetUserBookingsQueryHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<IEnumerable<TestDriveBookingDto>>> Handle(GetUserBookingsQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var bookings = await _context.TestDriveBookings.AsNoTracking()
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new TestDriveBookingDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    CarId = b.CarId,
                    CarTitle = b.Car != null ? b.Car.Title : null,
                    BookingDate = b.BookingDate,
                    Notes = b.Notes,
                    Status = b.Status.ToString(),
                    CreatedAt = b.CreatedAt
                }).ToListAsync(cancellationToken);

            return Result<IEnumerable<TestDriveBookingDto>>.Ok(bookings);
        }
    }
}
