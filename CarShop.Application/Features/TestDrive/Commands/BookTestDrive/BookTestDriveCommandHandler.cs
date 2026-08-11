using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using MediatR;
using System.Text.Json;
using CarEntity = CarShop.Domain.Entities.Car;

namespace CarShop.Application.Features.TestDrive.Commands.BookTestDrive
{
    public class BookTestDriveCommandHandler : IRequestHandler<BookTestDriveCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public BookTestDriveCommandHandler(IUnitOfWork unitOfWork, IAuditLogService auditLogService, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(BookTestDriveCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;

            var car = await _unitOfWork.Repository<CarEntity>().GetByIdAsync(request.CarId);
            if (car == null)
                return Result<string>.Fail("Car not found.");

            var booking = new TestDriveBooking
            {
                UserId = userId,
                CarId = request.CarId,
                BookingDate = request.BookingDate,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<TestDriveBooking>().AddAsync(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync("TestDrive", "Book",
                _userContextService.UserId, _userContextService.Email,
                $"Booked test drive for car '{car.Title}' on {request.BookingDate:yyyy-MM-dd}",
                entityId: booking.Id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                newValues: JsonSerializer.Serialize(new
                {
                    booking.CarId, CarTitle = car.Title,
                    BookingDate = request.BookingDate.ToString("yyyy-MM-dd HH:mm"),
                    booking.Notes, Status = booking.Status.ToString()
                }));

            return Result<string>.Ok(null, "Test drive booked successfully. We'll confirm your booking shortly.");
        }
    }
}
