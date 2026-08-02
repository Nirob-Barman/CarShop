using CarShop.Application.DTOs.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Commands.Login
{
    public class LoginCommand : IRequest<Result<string>>
    {
        public LoginDto Model { get; set; }

        public LoginCommand(LoginDto model)
        {
            Model = model;
        }
    }
}
