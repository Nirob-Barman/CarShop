using CarShop.Application.DTOs.Car;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Car.Queries.GetTopRatedCars
{
    public class GetTopRatedCarsQuery : IRequest<Result<IEnumerable<CarDto>>>
    {
        public int Count { get; set; }

        public GetTopRatedCarsQuery(int count = 4)
        {
            Count = count;
        }
    }
}
