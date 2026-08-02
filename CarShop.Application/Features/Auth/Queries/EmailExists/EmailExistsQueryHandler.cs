using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Queries.EmailExists
{
    public class EmailExistsQueryHandler : IRequestHandler<EmailExistsQuery, Result<bool>>
    {
        private readonly IUserManager _userManager;

        public EmailExistsQueryHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<bool>> Handle(EmailExistsQuery request, CancellationToken cancellationToken)
        {
            var exists = await _userManager.FindByEmailAsync(request.Email) != null;
            return Result<bool>.Ok(exists);
        }
    }
}
