using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Brand.Commands.UpdateBrand
{
    public class UpdateBrandCommand : IRequest<Result<string>>
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public UpdateBrandCommand(int id, string? name)
        {
            Id = id;
            Name = name;
        }
    }
}
