using CarShop.Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Brand.Commands.UpdateBrand
{
    public class UpdateBrandCommandValidator : AbstractValidator<UpdateBrandCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateBrandCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Brand name is required");

            RuleFor(x => x)
                .MustAsync(BeUniqueName).WithMessage("Another brand with this name already exists.")
                .When(x => !string.IsNullOrWhiteSpace(x.Name));
        }

        private async Task<bool> BeUniqueName(UpdateBrandCommand command, CancellationToken cancellationToken)
        {
            var exists = await _context.Brands.AnyAsync(b => b.Name == command.Name && b.Id != command.Id);
            return !exists;
        }
    }
}
