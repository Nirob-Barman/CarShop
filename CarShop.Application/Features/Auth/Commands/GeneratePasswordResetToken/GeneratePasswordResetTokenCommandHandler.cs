using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Commands.GeneratePasswordResetToken
{
    public class GeneratePasswordResetTokenCommandHandler : IRequestHandler<GeneratePasswordResetTokenCommand, Result<string>>
    {
        private readonly IUserManager _userManager;

        public GeneratePasswordResetTokenCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<string>> Handle(GeneratePasswordResetTokenCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return Result<string>.Fail("Email is required.");

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Result<string>.Fail("User not found.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return Result<string>.Ok(token);
        }
    }
}
