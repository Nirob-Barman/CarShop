using CarShop.Application.DTOs.Car;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Car.Queries.GetCarById
{
    public class GetCarByIdQuery : IRequest<Result<CarDto>>
    {
        public int Id { get; set; }

        public GetCarByIdQuery(int id)
        {
            Id = id;
        }
    }
}
