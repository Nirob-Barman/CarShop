using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Brand.Commands.DeleteBrand
{
    public record DeleteBrandCommand(int Id) : IRequest<Result<string>>;
}
