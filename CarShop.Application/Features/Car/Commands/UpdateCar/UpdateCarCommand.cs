using CarShop.Application.DTOs.Car;
using CarShop.Application.DTOs.File;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Car.Commands.UpdateCar
{
    public class UpdateCarCommand : IRequest<Result<string>>
    {
        public int Id { get; set; }
        public CarDto Dto { get; set; }
        public FileUploadDto? File { get; set; }

        public UpdateCarCommand(int id, CarDto dto, FileUploadDto? file)
        {
            Id = id;
            Dto = dto;
            File = file;
        }
    }
}
