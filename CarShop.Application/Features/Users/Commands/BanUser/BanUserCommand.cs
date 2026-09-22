using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Users.Commands.BanUser
{
    public record BanUserCommand(string UserId)
        : IRequest<Result<bool>>;
}
