using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.TestDrive.Commands.UpdateStatus
{
    public record UpdateStatusCommand(int BookingId, string Status)
        : IRequest<Result<string>>;
}
