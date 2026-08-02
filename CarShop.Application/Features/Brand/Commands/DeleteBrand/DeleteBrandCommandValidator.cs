using FluentValidation;

namespace CarShop.Application.Features.Brand.Commands.DeleteBrand
{
    public class DeleteBrandCommandValidator : AbstractValidator<DeleteBrandCommand>
    {
        public DeleteBrandCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("A valid brand id is required.");
        }
    }
}
