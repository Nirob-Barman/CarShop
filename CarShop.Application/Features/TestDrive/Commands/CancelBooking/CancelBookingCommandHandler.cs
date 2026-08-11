using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using MediatR;
using System.Text.Json;

namespace CarShop.Application.Features.TestDrive.Commands.CancelBooking
{
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public CancelBookingCommandHandler(IUnitOfWork unitOfWork, IAuditLogService auditLogService, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var booking = await _unitOfWork.Repository<TestDriveBooking>().FirstOrDefaultAsync(
                b => b.Id == request.BookingId && b.UserId == userId);

            if (booking == null)
                return Result<string>.Fail("Booking not found or you are not authorized to cancel it.");

            if (booking.Status == TestDriveStatus.Cancelled)
                return Result<string>.Fail("Booking is already cancelled.");

            var oldStatus = booking.Status;
            booking.Cancel();
            _unitOfWork.Repository<TestDriveBooking>().Update(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync("TestDrive", "Cancel",
                _userContextService.UserId, _userContextService.Email,
                entityId: request.BookingId,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                oldValues: JsonSerializer.Serialize(new { Status = oldStatus.ToString() }),
                newValues: JsonSerializer.Serialize(new { Status = "Cancelled" }));

            return Result<string>.Ok(null, "Test drive booking cancelled.");
        }
    }
}
