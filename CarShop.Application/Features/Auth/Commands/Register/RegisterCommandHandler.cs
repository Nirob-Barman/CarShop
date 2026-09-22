using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using MediatR;

namespace CarShop.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<string>>
    {
        private readonly IIdentityService _identityService;
        private readonly IEmailService _emailService;

        public RegisterCommandHandler(IIdentityService identityService, IEmailService emailService)
        {
            _identityService = identityService;
            _emailService = emailService;
        }

        public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = new AppUser
            {
                Email = request.Email,
                FullName = request.FullName,
                Address = request.Address
            };

            var (succeeded, userId, errors) = await _identityService.CreateAsync(user, request.Password!);

            if (!succeeded)
                return Result<string>.Fail(errors!, "Registration failed");

            var roleResult = await _identityService.AddToRoleAsync(new AppUser { Id = userId }, "User");

            if (!roleResult.Succeeded)
            {
                // Cleanup: delete user if role assignment fails
                await _identityService.RemoveFromRoleAsync(new AppUser { Id = userId }, "User");

                return Result<string>.Fail("Failed to assign default role to user.");
            }

            var welcomeMessage = $"Hello {request.FullName},<br>Welcome to CarShop! Thank you for registering.";
            await _emailService.SendEmailAsync(request.Email!, "Welcome to CarShop", welcomeMessage);

            return Result<string>.Ok(userId, "Registration successful");
        }
    }
}
