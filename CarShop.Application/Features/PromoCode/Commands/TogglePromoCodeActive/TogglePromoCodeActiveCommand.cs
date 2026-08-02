using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Commands.TogglePromoCodeActive
{
    public class TogglePromoCodeActiveCommand : IRequest<Result<string>>
    {
        public int Id { get; set; }

        public TogglePromoCodeActiveCommand(int id)
        {
            Id = id;
        }
    }
}
