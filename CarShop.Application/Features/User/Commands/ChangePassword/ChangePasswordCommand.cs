using CarShop.Application.DTOs.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.User.Commands.ChangePassword
{
    public record ChangePasswordCommand(ChangePasswordDto Model)
        : IRequest<Result<bool>>;
}
