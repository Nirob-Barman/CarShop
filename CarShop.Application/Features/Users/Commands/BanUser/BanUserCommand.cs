using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Users.Commands.BanUser
{
    public class BanUserCommand : IRequest<Result<bool>>
    {
        public string UserId { get; set; }

        public BanUserCommand(string userId)
        {
            UserId = userId;
        }
    }
}
