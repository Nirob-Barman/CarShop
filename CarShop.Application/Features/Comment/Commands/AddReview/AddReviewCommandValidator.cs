using CarShop.Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Comment.Commands.AddReview
{
    public class AddReviewCommandValidator : AbstractValidator<AddReviewCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public AddReviewCommandValidator(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Review content cannot be empty.");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

            RuleFor(x => x)
                .MustAsync(NotAlreadyReviewed).WithMessage("You have already reviewed this car.");
        }

        private async Task<bool> NotAlreadyReviewed(AddReviewCommand command, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId!;
            var alreadyReviewed = await _context.Comments.AnyAsync(
                c => c.CarId == command.CarId && c.UserId == userId && c.Rating.HasValue);
            return !alreadyReviewed;
        }
    }
}
