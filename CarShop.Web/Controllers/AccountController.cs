using CarShop.Application.DTOs.Identity;
using CarShop.Application.Interfaces;
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
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _userService.RegisterAsync(model);

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
        public async Task<IActionResult> Login(LoginDto model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            var result = await _userService.LoginAsync(model);

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

            return View(result.Data);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileDto model)
        {
            if (!ModelState.IsValid)
                return View("Profile", model);

            string userId = _userContextService.UserId!;

            var result = await _userService.UpdateProfileAsync(userId, model);

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
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string userId = _userContextService.UserId!;

            var result = await _userService.ChangePasswordAsync(userId, model);

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
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _userService.GeneratePasswordResetTokenAsync(model.Email!);

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
            return View(new ResetPasswordDto { Email = email, Token = token });
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _userService.ResetPasswordAsync(model.Email!, model.Token!, model.NewPassword!);

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
