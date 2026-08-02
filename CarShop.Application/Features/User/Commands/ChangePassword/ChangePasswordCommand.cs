using CarShop.Application.DTOs.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.User.Commands.ChangePassword
{
    public class ChangePasswordCommand : IRequest<Result<bool>>
    {
        public ChangePasswordDto Model { get; set; }

        public ChangePasswordCommand(ChangePasswordDto model)
        {
            Model = model;
        }
    }
}
