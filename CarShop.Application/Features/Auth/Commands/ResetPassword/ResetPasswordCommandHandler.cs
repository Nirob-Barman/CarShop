using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<bool>>
    {
        private readonly IIdentityService _identityService;

        public ResetPasswordCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return Result<bool>.Fail("Email, token, and new password are required.");
            }

            var user = await _identityService.FindByEmailAsync(request.Email);
            if (user == null)
                return Result<bool>.Fail("User not found.");

            var result = await _identityService.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (!result.Succeeded)
            {
                return Result<bool>.Fail(result.Errors, "Password reset failed.");
            }

            return Result<bool>.Ok(true, "Password has been reset successfully.");
        }
    }
}
