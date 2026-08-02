using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Users.Commands.BanUser
{
    public class BanUserCommandHandler : IRequestHandler<BanUserCommand, Result<bool>>
    {
        private readonly IUserManager _userManager;

        public BanUserCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<bool>> Handle(BanUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _userManager.SetLockoutAsync(request.UserId, ban: true);
            if (!result.Succeeded)
                return Result<bool>.Fail(result.Errors, "Failed to ban user.");
            return Result<bool>.Ok(true, "User has been banned.");
        }
    }
}
