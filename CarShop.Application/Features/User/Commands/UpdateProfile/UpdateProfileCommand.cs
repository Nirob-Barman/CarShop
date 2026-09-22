using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.User.Commands.UpdateProfile
{
    public record UpdateProfileCommand(
        string FullName,
        string Email,
        string Address) 
        : IRequest<Result<bool>>;
}
