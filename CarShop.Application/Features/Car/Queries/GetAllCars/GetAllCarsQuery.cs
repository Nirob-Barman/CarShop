using CarShop.Application.DTOs.Car;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Car.Queries.GetAllCars
{
    public class GetAllCarsQuery : IRequest<Result<IEnumerable<CarDto>>>
    {
    }
}
