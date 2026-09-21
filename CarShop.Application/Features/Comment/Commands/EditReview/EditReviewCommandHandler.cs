using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Comment.Commands.EditReview
{
    public class EditReviewCommandHandler : IRequestHandler<EditReviewCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public EditReviewCommandHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(EditReviewCommand request, CancellationToken cancellationToken)
        {
            var userId  = _userContextService.UserId!;
            var isAdmin = _userContextService.IsInRole("Admin");

            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == request.CommentId);
            if (comment == null)
                return Result<string>.Fail("Review not found.");

            if (!isAdmin && comment.UserId != userId)
                return Result<string>.Fail("You are not allowed to edit this review.");

            comment.Edit(request.Content!, request.Rating);
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "Review updated successfully.");
        }
    }
}
