using FluentValidation;

namespace CarShop.Application.Features.Comment.Commands.EditReview
{
    public class EditReviewCommandValidator : AbstractValidator<EditReviewCommand>
    {
        public EditReviewCommandValidator()
        {
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
        }
    }
}
