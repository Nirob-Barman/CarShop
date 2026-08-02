using CarShop.Application.DTOs.Comment;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using CommentEntity = CarShop.Domain.Entities.Comment;

namespace CarShop.Application.Features.Comment.Queries.GetCommentsByCarId
{
    public class GetCommentsByCarIdQueryHandler : IRequestHandler<GetCommentsByCarIdQuery, Result<IEnumerable<CommentDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCommentsByCarIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<CommentDto>>> Handle(GetCommentsByCarIdQuery request, CancellationToken cancellationToken)
        {
            var comments = await _unitOfWork.Repository<CommentEntity>().GetAllAsync(c => c.CarId == request.CarId, c => new CommentDto
            {
                Id = c.Id,
                UserName = c.UserName,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                CarId = c.CarId,
                Rating = c.Rating,
                UserId = c.UserId
            });

            return Result<IEnumerable<CommentDto>>.Ok(comments);
        }
    }
}
