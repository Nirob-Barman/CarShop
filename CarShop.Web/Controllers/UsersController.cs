using CarShop.Application.Interfaces;
using CarShop.Web.ViewModels.Admin;
using CarShop.Web.ViewModels.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var rolesResult = await _userService.GetAllRolesNonAdminAsync();
            var usersResult = await _userService.GetAllUsersNonAdminAsync();

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

            var result = await _userService.AssignRoleToUserAsync(userId, roleName);
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
            var result = await _userService.BanUserAsync(userId);
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
            var result = await _userService.UnbanUserAsync(userId);
            if (!result.Success)
                TempData["ErrorMessage"] = result.Message ?? "Failed to unban user.";
            else
                TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }
    }
}
