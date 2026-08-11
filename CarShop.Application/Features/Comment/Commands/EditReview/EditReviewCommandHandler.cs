using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using CommentEntity = CarShop.Domain.Entities.Comment;

namespace CarShop.Application.Features.Comment.Commands.EditReview
{
    public class EditReviewCommandHandler : IRequestHandler<EditReviewCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public EditReviewCommandHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(EditReviewCommand request, CancellationToken cancellationToken)
        {
            var userId  = _userContextService.UserId!;
            var isAdmin = _userContextService.IsInRole("Admin");

            var comment = await _unitOfWork.Repository<CommentEntity>().FirstOrDefaultAsync(c => c.Id == request.CommentId);
            if (comment == null)
                return Result<string>.Fail("Review not found.");

            if (!isAdmin && comment.UserId != userId)
                return Result<string>.Fail("You are not allowed to edit this review.");

            comment.Edit(request.Content!, request.Rating);
            _unitOfWork.Repository<CommentEntity>().Update(comment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "Review updated successfully.");
        }
    }
}
