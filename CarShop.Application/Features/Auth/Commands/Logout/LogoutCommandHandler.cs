using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<string>>
    {
        private readonly ISignInManager _signInManager;

        public LogoutCommandHandler(ISignInManager signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<Result<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            await _signInManager.SignOutAsync();
            return Result<string>.Ok("Success", "Logout successful");
        }
    }
}
