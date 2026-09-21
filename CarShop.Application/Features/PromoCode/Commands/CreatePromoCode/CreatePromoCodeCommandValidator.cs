using CarShop.Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.PromoCode.Commands.CreatePromoCode
{
    public class CreatePromoCodeCommandValidator : AbstractValidator<CreatePromoCodeCommand>
    {
        private readonly IApplicationDbContext _context;

        public CreatePromoCodeCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x)
                .MustAsync(BeUniqueCode).WithMessage("A promo code with this code already exists.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.Code));
        }

        private async Task<bool> BeUniqueCode(CreatePromoCodeCommand command, CancellationToken cancellationToken)
        {
            var exists = await _context.PromoCodes.AnyAsync(p => p.Code == command.Dto.Code.ToUpper());
            return !exists;
        }
    }
}
