using CarShop.Application.DTOs.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Commands.Register
{
    public class RegisterCommand : IRequest<Result<string>>
    {
        public RegisterDto Model { get; set; }

        public RegisterCommand(RegisterDto model)
        {
            Model = model;
        }
    }
}
