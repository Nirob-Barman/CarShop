using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Commands.Logout
{
    public record LogoutCommand : IRequest<Result<string>>;
}
