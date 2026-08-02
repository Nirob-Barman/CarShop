using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.PromoCode.Commands.DeletePromoCode
{
    public class DeletePromoCodeCommand : IRequest<Result<string>>
    {
        public int Id { get; set; }

        public DeletePromoCodeCommand(int id)
        {
            Id = id;
        }
    }
}
