using CarShop.Application.DTOs.Car;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Car.Queries.GetRecentCars
{
    public class GetRecentCarsQuery : IRequest<Result<IEnumerable<CarDto>>>
    {
        public int Count { get; set; }

        public GetRecentCarsQuery(int count = 4)
        {
            Count = count;
        }
    }
}
