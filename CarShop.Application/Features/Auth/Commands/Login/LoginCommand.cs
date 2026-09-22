using CarShop.Application.DTOs.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Commands.Login
{
    public record LoginCommand(LoginDto Model) : IRequest<Result<string>>;

}
