using CarShop.Application.DTOs.Car;
using CarShop.Application.DTOs.File;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Car.Commands.CreateCar
{
    public record CreateCarCommand(
        string Title,
        string? Description,
        decimal Price,
        int Quantity,
        int BrandId,
        FileUploadDto? File
    ) : IRequest<Result<int>>;
}
