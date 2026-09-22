using CarShop.Application.DTOs.Car;
using CarShop.Domain.Entities;
using System.Linq.Expressions;

namespace CarShop.Application.Mappers
{
    public static class CarMapper
    {
        public static readonly Expression<Func<Car, CarDto>> ToDtoExpression =
            car => new CarDto
            {
                Id = car.Id,
                Title = car.Title,
                Description = car.Description,
                Price = car.Price,
                Quantity = car.Quantity,
                ImageUrl = car.ImageUrl,
                BrandId = car.BrandId,
                BrandName = car.Brand != null
                    ? car.Brand.Name
                    : string.Empty
            };

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
    }
}
