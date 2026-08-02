using CarShop.Application.Interfaces.Persistence;
using FluentValidation;
using PaymentGatewayEntity = CarShop.Domain.Entities.PaymentGateway;

namespace CarShop.Application.Features.PaymentGateway.Commands.CreateGateway
{
    public class CreateGatewayCommandValidator : AbstractValidator<CreateGatewayCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateGatewayCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x)
                .MustAsync(BeUniqueSlug).WithMessage("A gateway with this slug already exists.");
        }

        private async Task<bool> BeUniqueSlug(CreateGatewayCommand command, CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.Repository<PaymentGatewayEntity>().AnyAsync(g => g.Slug == command.Dto.Slug);
            return !exists;
        }
    }
}
