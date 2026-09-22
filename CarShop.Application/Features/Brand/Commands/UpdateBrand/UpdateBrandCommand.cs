using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Brand.Commands.UpdateBrand
{
    public record UpdateBrandCommand(int Id, string? Name) : IRequest<Result<string>>;
}
