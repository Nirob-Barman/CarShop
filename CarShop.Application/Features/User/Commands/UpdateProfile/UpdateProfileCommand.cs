using CarShop.Application.DTOs.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.User.Commands.UpdateProfile
{
    public class UpdateProfileCommand : IRequest<Result<bool>>
    {
        public EditProfileDto Model { get; set; }

        public UpdateProfileCommand(EditProfileDto model)
        {
            Model = model;
        }
    }
}
