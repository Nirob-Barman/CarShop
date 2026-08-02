using CarShop.Application.Interfaces.Persistence;
using FluentValidation;
using PromoCodeEntity = CarShop.Domain.Entities.PromoCode;

namespace CarShop.Application.Features.PromoCode.Commands.CreatePromoCode
{
    public class CreatePromoCodeCommandValidator : AbstractValidator<CreatePromoCodeCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreatePromoCodeCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x)
                .MustAsync(BeUniqueCode).WithMessage("A promo code with this code already exists.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.Code));
        }

        private async Task<bool> BeUniqueCode(CreatePromoCodeCommand command, CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.Repository<PromoCodeEntity>().AnyAsync(p => p.Code == command.Dto.Code.ToUpper());
            return !exists;
        }
    }
}
