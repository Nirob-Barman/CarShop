using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Commands.Login
{
    public record LoginCommand(
        string? Email,
        string? Password,
        bool RememberMe)
        : IRequest<Result<string>>;

}
