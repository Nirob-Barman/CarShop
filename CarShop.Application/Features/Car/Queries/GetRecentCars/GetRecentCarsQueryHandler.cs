using CarShop.Application.DTOs.Car;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Mappers;
using CarShop.Application.Wrappers;
using MediatR;
using CarEntity = CarShop.Domain.Entities.Car;

namespace CarShop.Application.Features.Car.Queries.GetRecentCars
{
    public class GetRecentCarsQueryHandler : IRequestHandler<GetRecentCarsQuery, Result<IEnumerable<CarDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRecentCarsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<CarDto>>> Handle(GetRecentCarsQuery request, CancellationToken cancellationToken)
        {
            var cars = await _unitOfWork.Repository<CarEntity>().GetAllWithIncludesAsync(
                c => c, c => c.Brand!);

            var result = cars.OrderByDescending(c => c.Id).Take(request.Count).Select(CarMapper.ToDto);
            return Result<IEnumerable<CarDto>>.Ok(result);
        }
    }
}
