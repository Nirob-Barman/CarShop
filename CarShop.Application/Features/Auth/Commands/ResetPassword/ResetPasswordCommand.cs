using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommand : IRequest<Result<bool>>
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }

        public ResetPasswordCommand(string email, string token, string newPassword)
        {
            Email = email;
            Token = token;
            NewPassword = newPassword;
        }
    }
}
