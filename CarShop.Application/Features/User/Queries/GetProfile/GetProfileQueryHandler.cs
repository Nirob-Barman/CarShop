using CarShop.Application.DTOs.Identity;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.User.Queries.GetProfile
{
    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, Result<EditProfileDto>>
    {
        private readonly IUserManager _userManager;
        private readonly IUserContextService _userContextService;

        public GetProfileQueryHandler(IUserManager userManager, IUserContextService userContextService)
        {
            _userManager = userManager;
            _userContextService = userContextService;
        }

        public async Task<Result<EditProfileDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(_userContextService.UserId!);

            if (user == null)
                return Result<EditProfileDto>.Fail("User not found.");

            var dto = new EditProfileDto
            {
                FullName = user.FullName,
                Address = user.Address,
                Email = user.Email
            };

            return Result<EditProfileDto>.Ok(dto);
        }
    }
}
