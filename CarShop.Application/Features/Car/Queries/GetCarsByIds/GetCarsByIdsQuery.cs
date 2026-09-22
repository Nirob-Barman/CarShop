using CarShop.Application.DTOs.Car;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Car.Queries.GetCarsByIds
{
    public record GetCarsByIdsQuery(IEnumerable<int> Ids) : IRequest<Result<IEnumerable<CarDto>>>;
}
