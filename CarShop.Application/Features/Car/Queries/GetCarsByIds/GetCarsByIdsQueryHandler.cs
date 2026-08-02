using CarShop.Application.DTOs.Car;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Mappers;
using CarShop.Application.Wrappers;
using MediatR;
using CarEntity = CarShop.Domain.Entities.Car;

namespace CarShop.Application.Features.Car.Queries.GetCarsByIds
{
    public class GetCarsByIdsQueryHandler : IRequestHandler<GetCarsByIdsQuery, Result<IEnumerable<CarDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCarsByIdsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<CarDto>>> Handle(GetCarsByIdsQuery request, CancellationToken cancellationToken)
        {
            var idList = request.Ids.ToList();
            var cars = await _unitOfWork.Repository<CarEntity>().GetAllWithIncludesAsync(
                predicate: c => idList.Contains(c.Id),
                selector: c => c,
                c => c.Brand!
            );

            var result = cars.Select(CarMapper.ToDto);
            return Result<IEnumerable<CarDto>>.Ok(result);
        }
    }
}
