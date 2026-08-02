using CarShop.Application.DTOs.Car;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Mappers;
using CarShop.Application.Wrappers;
using MediatR;
using CarEntity = CarShop.Domain.Entities.Car;

namespace CarShop.Application.Features.Car.Queries.GetCarById
{
    public class GetCarByIdQueryHandler : IRequestHandler<GetCarByIdQuery, Result<CarDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCarByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CarDto>> Handle(GetCarByIdQuery request, CancellationToken cancellationToken)
        {
            var cars = await _unitOfWork.Repository<CarEntity>().GetAllWithIncludesAsync(
                predicate: c => c.Id == request.Id,
                selector: c => c,
                c => c.Brand!
            );
            var car = cars.FirstOrDefault();
            if (car == null)
                return Result<CarDto>.Fail("Car not found");

            return Result<CarDto>.Ok(CarMapper.ToDto(car));
        }
    }
}
