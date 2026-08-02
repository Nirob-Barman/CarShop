using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Brand.Commands.CreateBrand
{
    public class CreateBrandCommand : IRequest<Result<int>>
    {
        public string? Name { get; set; }

        public CreateBrandCommand(string? name)
        {
            Name = name;
        }
    }
}
