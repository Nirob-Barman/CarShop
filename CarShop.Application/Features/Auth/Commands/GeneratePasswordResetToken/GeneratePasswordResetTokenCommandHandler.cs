using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Commands.GeneratePasswordResetToken
{
    public class GeneratePasswordResetTokenCommandHandler : IRequestHandler<GeneratePasswordResetTokenCommand, Result<string>>
    {
        private readonly IIdentityService _identityService;
        private readonly IEmailService _emailService;

        public GeneratePasswordResetTokenCommandHandler(IIdentityService identityService, IEmailService emailService)
        {
            _identityService = identityService;
            _emailService = emailService;
        }

        public async Task<Result<string>> Handle(GeneratePasswordResetTokenCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return Result<string>.Fail("Email is required.");

            var user = await _identityService.FindByEmailAsync(request.Email);
            if (user == null)
                return Result<string>.Fail("User not found.");

            var token = await _identityService.GeneratePasswordResetTokenAsync(user);

            var resetLink =
                $"{request.BaseUrl.TrimEnd('/')}/Account/ResetPassword" +
                $"?email={Uri.EscapeDataString(request.Email)}" +
                $"&token={Uri.EscapeDataString(token)}";

            var encodedResetLink = System.Net.WebUtility.HtmlEncode(resetLink);

            var message = $"""
            <h2>Reset Your CarShop Password</h2>

            <p>You requested to reset your CarShop password.</p>

            <p>
                <a href="{encodedResetLink}">
                    Reset Password
                </a>
            </p>

            <p>
                If you did not request this, you can safely ignore
                this email.
            </p>
            """;
            
            try
            {
                await _emailService.SendEmailAsync(request.Email, "Reset Your CarShop Password", message);
                return Result<string>.Ok("Password reset email sent successfully.");
            }
            catch (Exception ex)
            {
                return Result<string>.Fail(ex.Message, "Unable to send password reset email. Please try again later.");
            }
        }
    }
}
