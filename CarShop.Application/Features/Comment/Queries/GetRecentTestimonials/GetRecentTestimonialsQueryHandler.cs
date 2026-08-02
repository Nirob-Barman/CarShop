using CarShop.Application.DTOs.Comment;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using CommentEntity = CarShop.Domain.Entities.Comment;

namespace CarShop.Application.Features.Comment.Queries.GetRecentTestimonials
{
    public class GetRecentTestimonialsQueryHandler : IRequestHandler<GetRecentTestimonialsQuery, Result<IEnumerable<CommentDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRecentTestimonialsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<CommentDto>>> Handle(GetRecentTestimonialsQuery request, CancellationToken cancellationToken)
        {
            var comments = await _unitOfWork.Repository<CommentEntity>().GetAllWithIncludesAsync(
                predicate: c => c.Rating.HasValue && c.Rating >= 4 && !string.IsNullOrEmpty(c.Content),
                selector: c => new CommentDto
                {
                    Id = c.Id,
                    UserName = c.UserName,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    CarId = c.CarId,
                    Rating = c.Rating,
                    UserId = c.UserId,
                    CarTitle = c.Car != null ? c.Car.Title : null
                },
                c => c.Car!
            );

            var result = comments.OrderByDescending(c => c.CreatedAt).Take(request.Count);
            return Result<IEnumerable<CommentDto>>.Ok(result);
        }
    }
}
