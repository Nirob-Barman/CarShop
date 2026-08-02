using CarShop.Application.DTOs.Car;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Car.Queries.GetCarsByIds
{
    public class GetCarsByIdsQuery : IRequest<Result<IEnumerable<CarDto>>>
    {
        public IEnumerable<int> Ids { get; set; }

        public GetCarsByIdsQuery(IEnumerable<int> ids)
        {
            Ids = ids;
        }
    }
}
