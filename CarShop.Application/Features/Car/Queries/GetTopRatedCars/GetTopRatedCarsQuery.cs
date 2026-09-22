using CarShop.Application.DTOs.Car;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Car.Queries.GetTopRatedCars
{
    public record GetTopRatedCarsQuery(int Count = 4) : IRequest<Result<IEnumerable<CarDto>>>;
}
