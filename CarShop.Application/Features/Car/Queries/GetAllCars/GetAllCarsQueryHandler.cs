using CarShop.Application.DTOs.Car;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Mappers;
using CarShop.Application.Wrappers;
using MediatR;
using CarEntity = CarShop.Domain.Entities.Car;

namespace CarShop.Application.Features.Car.Queries.GetAllCars
{
    public class GetAllCarsQueryHandler : IRequestHandler<GetAllCarsQuery, Result<IEnumerable<CarDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllCarsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<CarDto>>> Handle(GetAllCarsQuery request, CancellationToken cancellationToken)
        {
            var cars = await _unitOfWork.Repository<CarEntity>().GetAllWithIncludesAsync(
                selector: c => c,
                c => c.Brand!
            );
            var result = cars.Select(CarMapper.ToDto);
            return Result<IEnumerable<CarDto>>.Ok(result);
        }
    }
}
