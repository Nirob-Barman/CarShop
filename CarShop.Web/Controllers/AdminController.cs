using CarShop.Application.Interfaces;
using CarShop.Web.ViewModels.Admin;
using CarShop.Web.ViewModels.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }
        
        public async Task<IActionResult> Users()
        {
            var rolesResult = await _userService.GetAllRolesNonAdminAsync();
            var usersResult = await _userService.GetAllUsersNonAdminAsync();

            if (!usersResult.Success || usersResult.Data == null)
            {
                TempData["ErrorMessage"] = usersResult.Message ?? "Failed to retrieve users.";
                return View(new List<UserWithRoleViewModel>());
            }

            var vmList = UserMapper.ToViewModels(usersResult.Data);

            if (!rolesResult.Success || rolesResult.Data == null)
            {
                TempData["ErrorMessage"] = rolesResult.Message ?? "Failed to retrieve roles.";
                return View(vmList); // show users even if roles failed
            }

            foreach (var user in vmList)
            {
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
                return RedirectToAction("Users");
            }

            var result = await _userService.AssignRoleToUserAsync(userId, roleName);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message ?? "Failed to assign role.";
                return RedirectToAction("Users");
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Users");
        }


        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                ModelState.AddModelError("", "Role name is required.");
                return View();
            }

            var result = await _userService.CreateRoleAsync(roleName);

            if (!result.Success)
            {
                if (result.Errors != null)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error);
                }
                else if (!string.IsNullOrWhiteSpace(result.Message))
                {
                    ModelState.AddModelError("", result.Message);
                }

                return View();
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("CreateRole");
        }
    }
}
