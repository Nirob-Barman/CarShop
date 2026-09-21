using CarShop.Application.Interfaces;
using CarShop.Application.DTOs.Car;
using CarShop.Application.Mappers;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Car.Queries.GetAllCars
{
    public class GetAllCarsQueryHandler : IRequestHandler<GetAllCarsQuery, Result<IEnumerable<CarDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllCarsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IEnumerable<CarDto>>> Handle(GetAllCarsQuery request, CancellationToken cancellationToken)
        {
            var cars = await _context.Cars
                .AsNoTracking()
                .Include(c => c.Brand)
                .ToListAsync(cancellationToken);
            var result = cars.Select(CarMapper.ToDto);
            return Result<IEnumerable<CarDto>>.Ok(result);
        }
    }
}
