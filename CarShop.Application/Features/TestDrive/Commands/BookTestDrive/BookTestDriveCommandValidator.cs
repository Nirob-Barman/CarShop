using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using FluentValidation;
using CarShop.Domain.Entities;
using CarShop.Domain.Enums;

namespace CarShop.Application.Features.TestDrive.Commands.BookTestDrive
{
    public class BookTestDriveCommandValidator : AbstractValidator<BookTestDriveCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public BookTestDriveCommandValidator(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;

            RuleFor(x => x.BookingDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Booking date must be in the future.");

            RuleFor(x => x)
                .MustAsync(NotHaveRecentBooking).WithMessage("You already have a booking for this car within the last 7 days.");
        }

        private async Task<bool> NotHaveRecentBooking(BookTestDriveCommand command, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var recentBooking = await _unitOfWork.Repository<TestDriveBooking>().AnyAsync(
                b => b.UserId == userId && b.CarId == command.CarId &&
                     b.Status != TestDriveStatus.Cancelled &&
                     b.BookingDate >= DateTime.UtcNow.AddDays(-7));
            return !recentBooking;
        }
    }
}
