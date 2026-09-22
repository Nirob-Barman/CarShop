using CarShop.Application.DTOs.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Commands.Register
{
    public record RegisterCommand(RegisterDto Model) : IRequest<Result<string>>;
}
