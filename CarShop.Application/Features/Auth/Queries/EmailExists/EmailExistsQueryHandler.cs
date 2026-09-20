using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Queries.EmailExists
{
    public class EmailExistsQueryHandler : IRequestHandler<EmailExistsQuery, Result<bool>>
    {
        private readonly IIdentityService _identityService;

        public EmailExistsQueryHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<bool>> Handle(EmailExistsQuery request, CancellationToken cancellationToken)
        {
            var exists = await _identityService.FindByEmailAsync(request.Email) != null;
            return Result<bool>.Ok(exists);
        }
    }
}
