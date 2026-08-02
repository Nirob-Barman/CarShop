using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Users.Commands.UnbanUser
{
    public class UnbanUserCommandHandler : IRequestHandler<UnbanUserCommand, Result<bool>>
    {
        private readonly IUserManager _userManager;

        public UnbanUserCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<bool>> Handle(UnbanUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _userManager.SetLockoutAsync(request.UserId, ban: false);
            if (!result.Succeeded)
                return Result<bool>.Fail(result.Errors, "Failed to unban user.");
            return Result<bool>.Ok(true, "User has been unbanned.");
        }
    }
}
