using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Users.Commands.UnbanUser
{
    public class UnbanUserCommand : IRequest<Result<bool>>
    {
        public string UserId { get; set; }

        public UnbanUserCommand(string userId)
        {
            UserId = userId;
        }
    }
}
