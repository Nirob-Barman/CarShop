using CarShop.Application.DTOs.Car;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Car.Queries.GetCarById
{
    public record GetCarByIdQuery(int Id) : IRequest<Result<CarDto>>;
}
