using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.TestDrive.Commands.BookTestDrive
{
    public record BookTestDriveCommand(
        int CarId,
        DateTime BookingDate,
        string? Notes)
        : IRequest<Result<string>>;
}
