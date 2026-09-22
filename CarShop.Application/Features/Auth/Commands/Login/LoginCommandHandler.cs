using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<string>>
    {
        private readonly IIdentityService _identityService;

        public LoginCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.FindByEmailAsync(request.Email!);
            if (user == null)
                return Result<string>.FailField(nameof(request.Email), "This email is not registered.");

            if (user.IsBanned)
                return Result<string>.Fail("Your account has been banned. Please contact support.");

            var isPasswordValid = await _identityService.CheckPasswordSignInAsync(user, request.Password!);
            if (!isPasswordValid)
                return Result<string>.FailField(nameof(request.Password), "Incorrect password.");

            await _identityService.SignInAsync(user, isPersistent: request.RememberMe);

            return Result<string>.Ok("Success", "Login successful");
        }
    }
}
