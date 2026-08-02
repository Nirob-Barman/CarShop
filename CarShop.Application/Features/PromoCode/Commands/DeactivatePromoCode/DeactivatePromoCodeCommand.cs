using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Commands.DeactivatePromoCode
{
    public class DeactivatePromoCodeCommand : IRequest<Result<string>>
    {
        public int Id { get; set; }

        public DeactivatePromoCodeCommand(int id)
        {
            Id = id;
        }
    }
}
