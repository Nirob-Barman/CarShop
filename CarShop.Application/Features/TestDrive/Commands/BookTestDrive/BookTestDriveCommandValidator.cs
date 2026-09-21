using CarShop.Application.Interfaces;
using FluentValidation;
using CarShop.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.TestDrive.Commands.BookTestDrive
{
    public class BookTestDriveCommandValidator : AbstractValidator<BookTestDriveCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public BookTestDriveCommandValidator(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;

            RuleFor(x => x.BookingDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Booking date must be in the future.");

            RuleFor(x => x)
                .MustAsync(NotHaveRecentBooking).WithMessage("You already have a booking for this car within the last 7 days.");
        }

        private async Task<bool> NotHaveRecentBooking(BookTestDriveCommand command, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var recentBooking = await _context.TestDriveBookings.AnyAsync(
                b => b.UserId == userId && b.CarId == command.CarId &&
                     b.Status != TestDriveStatus.Cancelled &&
                     b.BookingDate >= DateTime.UtcNow.AddDays(-7));
            return !recentBooking;
        }
    }
}
