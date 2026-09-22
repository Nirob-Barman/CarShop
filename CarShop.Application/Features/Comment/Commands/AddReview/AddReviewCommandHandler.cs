using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using CommentEntity = CarShop.Domain.Entities.Comment;

namespace CarShop.Application.Features.Comment.Commands.AddReview
{
    public class AddReviewCommandHandler : IRequestHandler<AddReviewCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public AddReviewCommandHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(AddReviewCommand request, CancellationToken cancellationToken)
        {
            var userId   = _userContextService.UserId!;
            var userName = _userContextService.Email ?? "User";

            var comment = CommentEntity.Create(request.Content!, request.Rating, request.CarId, userId, userName);

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "Review added successfully.");
        }
    }
}
