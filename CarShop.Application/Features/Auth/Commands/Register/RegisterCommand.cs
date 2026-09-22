using CarShop.Application.DTOs.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Commands.Register
{
    public record RegisterCommand(
        string? FullName,
        string? Email,
        string? Password,
        string? Address) 
        : IRequest<Result<string>>;
}
