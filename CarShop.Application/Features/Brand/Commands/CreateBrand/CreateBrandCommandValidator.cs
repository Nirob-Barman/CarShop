using CarShop.Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Brand.Commands.CreateBrand
{
    public class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
    {
        private readonly IApplicationDbContext _context;

        public CreateBrandCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Brand name is required");

            RuleFor(x => x)
                .MustAsync(BeUniqueName).WithMessage("A brand with this name already exists.")
                .When(x => !string.IsNullOrWhiteSpace(x.Name));
        }

        private async Task<bool> BeUniqueName(CreateBrandCommand command, CancellationToken cancellationToken)
        {
            var exists = await _context.Brands.AnyAsync(b => b.Name == command.Name);
            return !exists;
        }
    }
}
