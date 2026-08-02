using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Brand.Commands.DeleteBrand
{
    public class DeleteBrandCommand : IRequest<Result<string>>
    {
        public int Id { get; set; }

        public DeleteBrandCommand(int id)
        {
            Id = id;
        }
    }
}
