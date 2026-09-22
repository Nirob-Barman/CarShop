using CarShop.Application.Interfaces;
using CarShop.Application.DTOs.Car;
using CarShop.Application.Mappers;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Car.Queries.GetRecentCars
{
    public class GetRecentCarsQueryHandler : IRequestHandler<GetRecentCarsQuery, Result<IEnumerable<CarDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetRecentCarsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IEnumerable<CarDto>>> Handle(GetRecentCarsQuery request, CancellationToken cancellationToken)
        {
            var cars = await _context.Cars.AsNoTracking()
                .OrderByDescending(c => c.Id)
                .Take(request.Count)
                .Select(CarMapper.ToDtoExpression)
                .ToListAsync(cancellationToken);

            return Result<IEnumerable<CarDto>>.Ok(cars);
        }
    }
}
