using CarShop.Application.DTOs.Car;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Car.Queries.SearchCars
{
    public class SearchCarsQuery : IRequest<Result<PagedResult<CarDto>>>
    {
        public CarSearchDto SearchDto { get; set; }

        public SearchCarsQuery(CarSearchDto searchDto)
        {
            SearchDto = searchDto;
        }
    }
}
