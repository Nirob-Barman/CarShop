using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.TestDrive.Commands.UpdateStatus
{
    public class UpdateStatusCommand : IRequest<Result<string>>
    {
        public int BookingId { get; set; }
        public string Status { get; set; }

        public UpdateStatusCommand(int bookingId, string status)
        {
            BookingId = bookingId;
            Status = status;
        }
    }
}
