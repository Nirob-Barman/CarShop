using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommand : IRequest<Result<string>>
    {
    }
}
