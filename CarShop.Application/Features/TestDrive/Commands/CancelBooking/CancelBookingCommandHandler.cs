using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using CarShop.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CarShop.Application.Features.TestDrive.Commands.CancelBooking
{
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public CancelBookingCommandHandler(IApplicationDbContext context, IAuditLogService auditLogService, IUserContextService userContextService)
        {
            _context = context;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var booking = await _context.TestDriveBookings.FirstOrDefaultAsync(
                b => b.Id == request.BookingId && b.UserId == userId);

            if (booking == null)
                return Result<string>.Fail("Booking not found or you are not authorized to cancel it.");

            if (booking.Status == TestDriveStatus.Cancelled)
                return Result<string>.Fail("Booking is already cancelled.");

            var oldStatus = booking.Status;
            booking.Cancel();
            await _context.SaveChangesAsync(cancellationToken);

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
