using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Comment.Commands.EditReview
{
    public class EditReviewCommand : IRequest<Result<string>>
    {
        public int CommentId { get; set; }
        public string? Content { get; set; }
        public int Rating { get; set; }

        public EditReviewCommand(int commentId, string? content, int rating)
        {
            CommentId = commentId;
            Content = content;
            Rating = rating;
        }
    }
}
