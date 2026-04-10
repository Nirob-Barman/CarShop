using CarShop.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly IUserService _userService;

        public RolesController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _userService.GetAllRolesAsync();
            if (!result.Success || result.Data == null)
            {
                TempData["ErrorMessage"] = result.Message ?? "Failed to retrieve roles.";
                return View(new List<string>());
            }
            return View(result.Data);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                ModelState.AddModelError("", "Role name is required.");
                return View();
            }

            var result = await _userService.CreateRoleAsync(roleName);
            if (!result.Success)
            {
                foreach (var error in result.Errors ?? new())
                    ModelState.AddModelError("", error);
                if (!string.IsNullOrWhiteSpace(result.Message))
                    ModelState.AddModelError("", result.Message);
                return View();
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Create");
        }

        [HttpGet]
        public IActionResult Edit(string roleName)
        {
            if (string.IsNullOrEmpty(roleName)) return RedirectToAction("Index");
            return View(model: roleName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string currentName, string newName)
        {
            var result = await _userService.RenameRoleAsync(currentName, newName);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return View(model: currentName);
            }
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string roleName)
        {
            var result = await _userService.DeleteRoleAsync(roleName);
            if (!result.Success)
                TempData["ErrorMessage"] = result.Message;
            else
                TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }
    }
}
