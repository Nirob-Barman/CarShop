using CarShop.Application.Features.Roles.Queries.GetAllRolesNonAdmin;
using CarShop.Application.Features.Users.Commands.AssignRoleToUser;
using CarShop.Application.Features.Users.Commands.BanUser;
using CarShop.Application.Features.Users.Commands.UnbanUser;
using CarShop.Application.Features.Users.Queries.GetAllUsersNonAdmin;
using CarShop.Web.ViewModels.Admin;
using CarShop.Web.ViewModels.Mappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var rolesResult = await _mediator.Send(new GetAllRolesNonAdminQuery());
            var usersResult = await _mediator.Send(new GetAllUsersNonAdminQuery());

            if (!usersResult.Success || usersResult.Data == null)
            {
                TempData["ErrorMessage"] = usersResult.Message ?? "Failed to retrieve users.";
                return View(new List<UserWithRoleViewModel>());
            }

            var vmList = UserMapper.ToViewModels(usersResult.Data);

            if (rolesResult.Success && rolesResult.Data != null)
            {
                foreach (var user in vmList)
                    user.AllRoles = rolesResult.Data;
            }

            return View(vmList);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
            {
                TempData["ErrorMessage"] = "User ID and Role are required.";
                return RedirectToAction("Index");
            }

            var result = await _mediator.Send(new AssignRoleToUserCommand(userId, roleName));
            if (!result.Success)
                TempData["ErrorMessage"] = result.Message ?? "Failed to assign role.";
            else
                TempData["SuccessMessage"] = result.Message;

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ban(string userId)
        {
            var result = await _mediator.Send(new BanUserCommand(userId));
            if (!result.Success)
                TempData["ErrorMessage"] = result.Message ?? "Failed to ban user.";
            else
                TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unban(string userId)
        {
            var result = await _mediator.Send(new UnbanUserCommand(userId));
            if (!result.Success)
                TempData["ErrorMessage"] = result.Message ?? "Failed to unban user.";
            else
                TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }
    }
}
