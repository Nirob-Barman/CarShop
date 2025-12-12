using CarShop.Application.DTOs.Identity;
using CarShop.Application.Interfaces;
using CarShop.Web.ViewModels.Account;
using CarShop.Web.ViewModels.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserContextService _userContextService;
        public AccountController(IUserService userService, IUserContextService userContextService)
        {
            _userService = userService;
            _userContextService = userContextService;
        }

        public IActionResult Register()
        {

            if (_userContextService.IsAuthenticated)
            {
                return RedirectToAction("Profile");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = AccountMapper.ToDto(model);
            var result = await _userService.RegisterAsync(dto);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return View(model);
            }

            //TempData["SuccessMessage"] = "Registration successful. You can now log in.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (_userContextService.IsAuthenticated)
            {
                return RedirectToLocal(returnUrl);
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            var dto = AccountMapper.ToDto(model);
            var result = await _userService.LoginAsync(dto);

            if (!result.Success)
            {
                if (result.FieldErrors != null)
                {
                    foreach (var kvp in result.FieldErrors)
                    {
                        ModelState.AddModelError(kvp.Key, kvp.Value);
                    }
                }

                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            return RedirectToLocal(returnUrl);
        }


        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var result = await _userService.LogoutAsync();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> CheckEmailExists(string email)
        {
            bool emailExists = await _userService.EmailExistsAsync(email);
            return Json(emailExists);
        }


        [Authorize]
        public async Task<IActionResult> Profile()
        {
            string userId = _userContextService.UserId!;

            var result = await _userService.GetProfileAsync(userId);

            if (!result.Success || result.Data == null)
            {
                TempData["ErrorMessage"] = result.Message ?? "Failed to load profile.";
                return RedirectToAction("Login");
            }

            var vm = AccountMapper.ToViewModel(result.Data);
            return View(vm);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditProfile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Profile", model);

            var dto = AccountMapper.ToDto(model);
            string userId = _userContextService.UserId!;

            var result = await _userService.UpdateProfileAsync(userId, dto);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message ?? "Profile update failed.";
                return View("Profile", model);
            }

            TempData["SuccessMessage"] = result.Message ?? "Profile updated.";
            return RedirectToAction("Profile");
        }


        [Authorize]
        public IActionResult ChangePassword() => View();


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string userId = _userContextService.UserId!;
            var dto = AccountMapper.ToDto(model);
            var result = await _userService.ChangePasswordAsync(userId, dto);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Profile");
            }
            else
            {
                if (result.Errors != null)
                {
                    if (result.FieldErrors != null)
                    {
                        foreach (var kvp in result.FieldErrors)
                        {
                            ModelState.AddModelError(kvp.Key, kvp.Value);
                        }
                    }
                }

                TempData["ErrorMessage"] = result.Message ?? "An error occurred.";
                return View(model);
            }
        }


        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = AccountMapper.ToDto(model);
            var result = await _userService.GeneratePasswordResetTokenAsync(dto.Email!);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = string.Join(", ", result.Errors!);
                return View(model);
            }

            var token = result.Data;
            //It generates a full absolute URL(including protocol and domain) that points to the ResetPassword action on your AccountController, with query parameters for the email and token.
            //Request.Scheme Ensures the generated link uses http:// or https:// depending on the request. This makes the URL absolute.
            var resetLink = Url.Action("ResetPassword","Account", new { email = model.Email, token = token }, Request.Scheme );

            // Simulate sending email
            TempData["ResetLink"] = resetLink;
            ViewBag.Message = "Reset link generated. (Check email)";

            return View();
        }


        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            return View(new ResetPasswordViewModel { Email = email, Token = token });
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = AccountMapper.ToDto(model);
            var result = await _userService.ResetPasswordAsync(dto.Email!, dto.Token!, dto.NewPassword!);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = string.Join("; ", result.Errors!);
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied(string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
