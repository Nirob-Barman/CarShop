using CarShop.Application.Interfaces.Persistence;
using FluentValidation;
using BrandEntity = CarShop.Domain.Entities.Brand;

namespace CarShop.Application.Features.Brand.Commands.CreateBrand
{
    public class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateBrandCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Brand name is required");

            RuleFor(x => x)
                .MustAsync(BeUniqueName).WithMessage("A brand with this name already exists.")
                .When(x => !string.IsNullOrWhiteSpace(x.Name));
        }

        private async Task<bool> BeUniqueName(CreateBrandCommand command, CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.Repository<BrandEntity>().AnyAsync(b => b.Name == command.Name);
            return !exists;
        }
    }
}
