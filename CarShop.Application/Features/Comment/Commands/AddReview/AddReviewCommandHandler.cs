using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using CommentEntity = CarShop.Domain.Entities.Comment;

namespace CarShop.Application.Features.Comment.Commands.AddReview
{
    public class AddReviewCommandHandler : IRequestHandler<AddReviewCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public AddReviewCommandHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(AddReviewCommand request, CancellationToken cancellationToken)
        {
            var userId   = _userContextService.UserId!;
            var userName = _userContextService.Email ?? "User";

            var comment = new CommentEntity
            {
                CarId = request.CarId,
                UserId = userId,
                UserName = userName.Trim(),
                Content = request.Content!.Trim(),
                Rating = request.Rating,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<CommentEntity>().AddAsync(comment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "Review added successfully.");
        }
    }
}
