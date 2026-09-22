using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Commands.GeneratePasswordResetToken
{
    public record GeneratePasswordResetTokenCommand(string Email) : IRequest<Result<string>>;
}
