using CarShop.Application.DTOs.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.User.Commands.UpdateProfile
{
    public record UpdateProfileCommand(EditProfileDto Model)
        : IRequest<Result<bool>>;
}
