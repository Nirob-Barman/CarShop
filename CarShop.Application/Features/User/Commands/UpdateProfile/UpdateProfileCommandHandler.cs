using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.User.Commands.UpdateProfile
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result<bool>>
    {
        private readonly IIdentityService _identityService;
        private readonly IUserContextService _userContextService;

        public UpdateProfileCommandHandler(IIdentityService identityService, IUserContextService userContextService)
        {
            _identityService = identityService;
            _userContextService = userContextService;
        }

        public async Task<Result<bool>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.FindByIdAsync(_userContextService.UserId!);
            if (user == null)
                return Result<bool>.Fail("User not found.");

            user.FullName = request.FullName;
            user.Address = request.Address;

            var updateResult = await _identityService.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return Result<bool>.Fail(updateResult.Errors, "Failed to update profile.");
            }

            return Result<bool>.Ok(true, "Profile updated successfully.");
        }
    }
}
