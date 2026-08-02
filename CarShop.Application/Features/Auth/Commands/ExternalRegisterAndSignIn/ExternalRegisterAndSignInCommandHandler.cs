using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using MediatR;

namespace CarShop.Application.Features.Auth.Commands.ExternalRegisterAndSignIn
{
    public class ExternalRegisterAndSignInCommandHandler : IRequestHandler<ExternalRegisterAndSignInCommand, Result<string>>
    {
        private readonly IUserManager _userManager;
        private readonly ISignInManager _signInManager;
        private readonly IEmailService _emailService;

        public ExternalRegisterAndSignInCommandHandler(IUserManager userManager, ISignInManager signInManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public async Task<Result<string>> Handle(ExternalRegisterAndSignInCommand request, CancellationToken cancellationToken)
        {
            var user = new AppUser
            {
                Email = request.Email,
                FullName = string.IsNullOrWhiteSpace(request.FullName) ? request.Email : request.FullName
            };

            var (succeeded, userId, errors) = await _userManager.CreateWithoutPasswordAsync(user);
            if (!succeeded)
                return Result<string>.Fail(errors, "Failed to create account.");

            await _userManager.AddToRoleAsync(new AppUser { Id = userId }, "User");
            await _userManager.AddLoginAsync(userId!, request.Provider, request.ProviderKey);
            await _signInManager.SignInAsync(new AppUser { Id = userId }, isPersistent: false);

            var welcomeMessage = $"Hello {user.FullName},<br>Welcome to CarShop! Your account was created via Google sign-in.";
            await _emailService.SendEmailAsync(request.Email, "Welcome to CarShop", welcomeMessage);

            return Result<string>.Ok(userId, "Account created successfully.");
        }
    }
}
