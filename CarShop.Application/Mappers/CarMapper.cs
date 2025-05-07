using CarShop.Application.DTOs.Car;
using CarShop.Domain.Entities;

namespace CarShop.Application.Mappers
{
    public static class CarMapper
    {
        public static CarDto ToDto(Car car)
        {
            return new CarDto
            {
                Id = car.Id,
                Title = car.Title,
                Description = car.Description,
                Price = car.Price,
                Quantity = car.Quantity,
                ImageUrl = car.ImageUrl,
                BrandId = car.BrandId,
                BrandName = car.Brand?.Name ?? string.Empty
            };
        }

        public static Car ToEntity(CarDto dto)
        {
            return new Car
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                Quantity = dto.Quantity,
                ImageUrl = dto.ImageUrl,
                BrandId = dto.BrandId
            };
        }

        public static void UpdateEntity(Car car, CarDto dto)
        {
            car.Title = dto.Title;
            car.Description = dto.Description;
            car.Price = dto.Price;
            car.Quantity = dto.Quantity;
            car.ImageUrl = dto.ImageUrl;
            car.BrandId = dto.BrandId;
        }
    }
}
