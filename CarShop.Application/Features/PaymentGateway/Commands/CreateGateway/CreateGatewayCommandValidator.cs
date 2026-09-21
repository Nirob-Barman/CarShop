using CarShop.Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.PaymentGateway.Commands.CreateGateway
{
    public class CreateGatewayCommandValidator : AbstractValidator<CreateGatewayCommand>
    {
        private readonly IApplicationDbContext _context;

        public CreateGatewayCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x)
                .MustAsync(BeUniqueSlug).WithMessage("A gateway with this slug already exists.");
        }

        private async Task<bool> BeUniqueSlug(CreateGatewayCommand command, CancellationToken cancellationToken)
        {
            var exists = await _context.PaymentGateways.AnyAsync(g => g.Slug == command.Dto.Slug);
            return !exists;
        }
    }
}
