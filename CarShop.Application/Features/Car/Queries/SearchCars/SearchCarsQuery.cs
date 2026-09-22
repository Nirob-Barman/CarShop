using CarShop.Application.DTOs.Car;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Car.Queries.SearchCars
{
    public record SearchCarsQuery(CarSearchDto SearchDto) : IRequest<Result<PagedResult<CarDto>>>;
}
