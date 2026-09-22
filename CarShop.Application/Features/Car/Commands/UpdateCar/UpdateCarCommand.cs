using CarShop.Application.DTOs.File;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Car.Commands.UpdateCar
{
    public record UpdateCarCommand(
        int Id,
        string Title,
        string? Description,
        decimal Price,
        int Quantity,
        int BrandId,
        FileUpload? File
    ) : IRequest<Result<string>>;
}
