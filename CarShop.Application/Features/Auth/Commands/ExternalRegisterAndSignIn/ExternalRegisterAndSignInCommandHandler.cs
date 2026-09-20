using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Commands.ExternalRegisterAndSignIn
{
    public class ExternalRegisterAndSignInCommandHandler : IRequestHandler<ExternalRegisterAndSignInCommand, Result<string>>
    {
        private readonly IIdentityService _identityService;
        private readonly IEmailService _emailService;

        public ExternalRegisterAndSignInCommandHandler(IIdentityService identityService, IEmailService emailService)
        {
            _identityService = identityService;
            _emailService = emailService;
        }

        public async Task<Result<string>> Handle(ExternalRegisterAndSignInCommand request, CancellationToken cancellationToken)
        {
            var fullName = string.IsNullOrWhiteSpace(request.FullName) ? request.Email : request.FullName;
            var (succeeded, isNewUser, userId, errors) = await _identityService.ExternalLoginSignInAsync(
                request.Email, fullName, request.Provider, request.ProviderKey, request.Provider);
            if (!succeeded)
                return Result<string>.Fail(errors, "Could not sign in with Google.");

            if (isNewUser)
            {
                var welcomeMessage = $"Hello {fullName},<br>Welcome to CarShop! Your account was created via Google sign-in.";
                await _emailService.SendEmailAsync(request.Email, "Welcome to CarShop", welcomeMessage);
            }

            return Result<string>.Ok(userId, isNewUser ? "Account created successfully." : "Login successful.");
        }
    }
}
