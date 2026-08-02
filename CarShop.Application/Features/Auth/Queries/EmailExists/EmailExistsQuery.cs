using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Queries.EmailExists
{
    public class EmailExistsQuery : IRequest<Result<bool>>
    {
        public string Email { get; set; }

        public EmailExistsQuery(string email)
        {
            Email = email;
        }
    }
}
