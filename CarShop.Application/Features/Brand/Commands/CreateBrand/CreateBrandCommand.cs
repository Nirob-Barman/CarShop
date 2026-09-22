using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Brand.Commands.CreateBrand
{
    public record CreateBrandCommand(string? Name) : IRequest<Result<int>>;
}
