using CarShop.Application.Interfaces;
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
            var allRoles = await _userService.GetAllRolesNonAdminAsync();
            var users = await _userService.GetAllUsersNonAdminAsync();

            foreach (var user in users)
            {
                user.AllRoles = allRoles;
            }

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
            {
                TempData["ErrorMessage"] = "User ID and Role are required.";
                return RedirectToAction("Users");
            }

            try
            {
                await _userService.AssignRoleToUserAsync(userId, roleName);
                TempData["SuccessMessage"] = $"Role {roleName} updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

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

            try
            {
                await _userService.CreateRoleAsync(roleName);
                TempData["SuccessMessage"] = $"Role '{roleName}' created successfully.";
                return RedirectToAction("CreateRole");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }
        }
    }
}
