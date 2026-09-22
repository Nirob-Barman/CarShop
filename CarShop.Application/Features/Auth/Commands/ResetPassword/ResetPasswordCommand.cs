using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Commands.ResetPassword
{
    public record ResetPasswordCommand(
        string Email,
        string Token,
        string NewPassword
    ) : IRequest<Result<bool>>;
}
