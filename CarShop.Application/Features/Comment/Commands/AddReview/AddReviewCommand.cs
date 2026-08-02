using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Comment.Commands.AddReview
{
    public class AddReviewCommand : IRequest<Result<string>>
    {
        public int CarId { get; set; }
        public string? Content { get; set; }
        public int Rating { get; set; }

        public AddReviewCommand(int carId, string? content, int rating)
        {
            CarId = carId;
            Content = content;
            Rating = rating;
        }
    }
}
