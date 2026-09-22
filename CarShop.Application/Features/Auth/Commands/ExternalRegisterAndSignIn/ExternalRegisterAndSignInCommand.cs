using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Commands.ExternalRegisterAndSignIn
{
    public record ExternalRegisterAndSignInCommand(
        string Email,
        string FullName,
        string Provider,
        string ProviderKey
    ) : IRequest<Result<string>>;
}
