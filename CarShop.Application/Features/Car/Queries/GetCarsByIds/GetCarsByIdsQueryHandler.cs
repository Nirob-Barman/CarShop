using CarShop.Application.Interfaces;
using CarShop.Application.DTOs.Car;
using CarShop.Application.Mappers;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Car.Queries.GetCarsByIds
{
    public class GetCarsByIdsQueryHandler : IRequestHandler<GetCarsByIdsQuery, Result<IEnumerable<CarDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetCarsByIdsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IEnumerable<CarDto>>> Handle(GetCarsByIdsQuery request, CancellationToken cancellationToken)
        {
            var cars = await _context.Cars.AsNoTracking()
                .Include(c => c.Brand)
                .Where(c => request.Ids.Contains(c.Id))
                .Select(CarMapper.ToDtoExpression)
                .ToListAsync(cancellationToken);

            return Result<IEnumerable<CarDto>>.Ok(cars);
        }
    }
}
