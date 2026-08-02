using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Comment.Commands.DeleteReview
{
    public class DeleteReviewCommand : IRequest<Result<string>>
    {
        public int CommentId { get; set; }

        public DeleteReviewCommand(int commentId)
        {
            CommentId = commentId;
        }
    }
}
