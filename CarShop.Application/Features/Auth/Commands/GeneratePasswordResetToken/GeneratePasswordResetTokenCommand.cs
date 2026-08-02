using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Commands.GeneratePasswordResetToken
{
    public class GeneratePasswordResetTokenCommand : IRequest<Result<string>>
    {
        public string Email { get; set; }

        public GeneratePasswordResetTokenCommand(string email)
        {
            Email = email;
        }
    }
}
