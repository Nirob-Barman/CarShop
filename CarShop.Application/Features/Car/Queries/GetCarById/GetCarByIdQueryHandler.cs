using CarShop.Application.Interfaces;
using CarShop.Application.DTOs.Car;
using CarShop.Application.Mappers;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Car.Queries.GetCarById
{
    public class GetCarByIdQueryHandler : IRequestHandler<GetCarByIdQuery, Result<CarDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetCarByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<CarDto>> Handle(GetCarByIdQuery request, CancellationToken cancellationToken)
        {
            var car = await _context.Cars.AsNoTracking()
                .Include(c => c.Brand)
                .Where(c => c.Id == request.Id)
                .Select(CarMapper.ToDtoExpression)
                .FirstOrDefaultAsync(cancellationToken);

            if (car == null)
                return Result<CarDto>.Fail("Car not found");

            return Result<CarDto>.Ok(car);
        }
    }
}
