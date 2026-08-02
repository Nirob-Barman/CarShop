using CarShop.Application.DTOs.Car;
using CarShop.Application.DTOs.File;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Car.Commands.CreateCar
{
    public class CreateCarCommand : IRequest<Result<int>>
    {
        public CarDto Dto { get; set; }
        public FileUploadDto? File { get; set; }

        public CreateCarCommand(CarDto dto, FileUploadDto? file)
        {
            Dto = dto;
            File = file;
        }
    }
}
