using CarShop.Application.DTOs.Car;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Mappers;
using CarShop.Application.Wrappers;
using MediatR;
using CarEntity = CarShop.Domain.Entities.Car;
using CommentEntity = CarShop.Domain.Entities.Comment;

namespace CarShop.Application.Features.Car.Queries.GetTopRatedCars
{
    public class GetTopRatedCarsQueryHandler : IRequestHandler<GetTopRatedCarsQuery, Result<IEnumerable<CarDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTopRatedCarsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<CarDto>>> Handle(GetTopRatedCarsQuery request, CancellationToken cancellationToken)
        {
            var count = request.Count;

            // Load all comments with ratings, group by car, compute average
            var comments = await _unitOfWork.Repository<CommentEntity>().GetAllAsync(
                c => c.Rating.HasValue,
                c => new { c.CarId, c.Rating }
            );

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
                var newest = await _unitOfWork.Repository<CarEntity>().GetAllWithIncludesAsync(
                    c => c.Quantity > 0, c => c, c => c.Brand!);
                return Result<IEnumerable<CarDto>>.Ok(
                    newest.OrderByDescending(c => c.Id).Take(count).Select(CarMapper.ToDto));
            }

            var cars = await _unitOfWork.Repository<CarEntity>().GetAllWithIncludesAsync(
                predicate: c => topCarIds.Contains(c.Id),
                selector: c => c,
                c => c.Brand!
            );

            return Result<IEnumerable<CarDto>>.Ok(cars.Select(CarMapper.ToDto));
        }
    }
}
