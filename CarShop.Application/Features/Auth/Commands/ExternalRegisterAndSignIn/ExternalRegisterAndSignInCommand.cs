using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Commands.ExternalRegisterAndSignIn
{
    public class ExternalRegisterAndSignInCommand : IRequest<Result<string>>
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Provider { get; set; }
        public string ProviderKey { get; set; }

        public ExternalRegisterAndSignInCommand(string email, string fullName, string provider, string providerKey)
        {
            Email = email;
            FullName = fullName;
            Provider = provider;
            ProviderKey = providerKey;
        }
    }
}
