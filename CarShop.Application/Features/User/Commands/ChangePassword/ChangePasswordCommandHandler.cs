using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.User.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<bool>>
    {
        private readonly IUserManager _userManager;
        private readonly ISignInManager _signInManager;
        private readonly IUserContextService _userContextService;

        public ChangePasswordCommandHandler(IUserManager userManager, ISignInManager signInManager, IUserContextService userContextService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userContextService = userContextService;
        }

        public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;

            if (string.IsNullOrWhiteSpace(model.CurrentPassword))
            {
                return Result<bool>.FailField(nameof(model.CurrentPassword), "Password fields cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(model.NewPassword))
            {
                return Result<bool>.FailField(nameof(model.NewPassword), "Password fields cannot be empty.");
            }

            var user = await _userManager.FindByIdAsync(_userContextService.UserId!);
            if (user == null)
                return Result<bool>.Fail("User not found.");

            var result = await _userManager.ChangePasswordAsync(user.Id!, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return Result<bool>.Fail(result.Errors, "Password change failed.");
            }

            // Re-sign in to refresh security stamp/cookies
            await _signInManager.RefreshSignInAsync(user);

            return Result<bool>.Ok(true, "Password changed successfully.");
        }
    }
}
