using CarShop.Application.DTOs.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.User.Queries.GetProfile
{
    public record GetProfileQuery
        : IRequest<Result<EditProfileDto>>;
}
