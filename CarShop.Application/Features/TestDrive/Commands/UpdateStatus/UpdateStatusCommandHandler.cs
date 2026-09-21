using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using CarShop.Domain.Enums;
using MediatR;
using System.Text.Json;

namespace CarShop.Application.Features.TestDrive.Commands.UpdateStatus
{
    public class UpdateStatusCommandHandler : IRequestHandler<UpdateStatusCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public UpdateStatusCommandHandler(IApplicationDbContext context, IAuditLogService auditLogService, IUserContextService userContextService)
        {
            _context = context;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
        {
            var booking = await _context.TestDriveBookings.FindAsync(request.BookingId);
            if (booking == null)
                return Result<string>.Fail("Booking not found.");

            if (!Enum.TryParse<TestDriveStatus>(request.Status, ignoreCase: true, out var newStatus))
                return Result<string>.Fail("Invalid status.");

            var oldStatus = booking.Status;
            var changed = newStatus switch
            {
                TestDriveStatus.Confirmed => booking.Confirm(),
                TestDriveStatus.Cancelled => booking.Cancel(),
                _ => false
            };
            if (!changed)
                return Result<string>.Fail("Could not update status.");

            _context.TestDriveBookings.Update(booking);
            await _context.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync("TestDrive", "StatusUpdate",
                _userContextService.UserId, _userContextService.Email,
                $"Status changed from '{oldStatus}' to '{newStatus}'",
                entityId: request.BookingId,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                oldValues: JsonSerializer.Serialize(new { Status = oldStatus.ToString() }),
                newValues: JsonSerializer.Serialize(new { Status = newStatus.ToString() }));

            return Result<string>.Ok(null, $"Booking status updated to {newStatus}.");
        }
    }
}
