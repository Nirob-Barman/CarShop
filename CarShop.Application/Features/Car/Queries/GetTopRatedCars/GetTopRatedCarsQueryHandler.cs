using CarShop.Application.Interfaces;
using CarShop.Application.DTOs.Car;
using CarShop.Application.Mappers;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Car.Queries.GetTopRatedCars
{
    public class GetTopRatedCarsQueryHandler : IRequestHandler<GetTopRatedCarsQuery, Result<IEnumerable<CarDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetTopRatedCarsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IEnumerable<CarDto>>> Handle(GetTopRatedCarsQuery request, CancellationToken cancellationToken)
        {
            var count = request.Count;

            // Load all comments with ratings, group by car, compute average
            var comments = await _context.Comments.AsNoTracking()
                .Where(c => c.Rating.HasValue)
                .Select(c => new { c.CarId, c.Rating })
                .ToListAsync(cancellationToken);

            var topCarIds = comments
                .GroupBy(c => c.CarId)
                .Select(g => new { CarId = g.Key, AvgRating = g.Average(c => c.Rating!.Value) })
                .OrderByDescending(x => x.AvgRating)
                .Take(count)
                .Select(x => x.CarId)
                .ToList();

            if (!topCarIds.Any())
            {
                // Fall back to newest cars when no ratings exist yet
                var newest = await _context.Cars.AsNoTracking().Include(c => c.Brand)
                    .Where(c => c.Quantity > 0).ToListAsync(cancellationToken);
                return Result<IEnumerable<CarDto>>.Ok(
                    newest.OrderByDescending(c => c.Id).Take(count).Select(CarMapper.ToDto));
            }

            var cars = await _context.Cars.AsNoTracking().Include(c => c.Brand)
                .Where(c => topCarIds.Contains(c.Id)).ToListAsync(cancellationToken);

            return Result<IEnumerable<CarDto>>.Ok(cars.Select(CarMapper.ToDto));
        }
    }
}
