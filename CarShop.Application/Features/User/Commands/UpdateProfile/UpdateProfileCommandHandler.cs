using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.User.Commands.UpdateProfile
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result<bool>>
    {
        private readonly IUserManager _userManager;
        private readonly IUserContextService _userContextService;

        public UpdateProfileCommandHandler(IUserManager userManager, IUserContextService userContextService)
        {
            _userManager = userManager;
            _userContextService = userContextService;
        }

        public async Task<Result<bool>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(_userContextService.UserId!);
            if (user == null)
                return Result<bool>.Fail("User not found.");

            user.FullName = request.Model.FullName;
            user.Address = request.Model.Address;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return Result<bool>.Fail(updateResult.Errors, "Failed to update profile.");
            }

            return Result<bool>.Ok(true, "Profile updated successfully.");
        }
    }
}
