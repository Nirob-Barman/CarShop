using CarShop.Application.Features.Auth.Commands.ExternalRegisterAndSignIn;
using CarShop.Application.Features.Auth.Commands.GeneratePasswordResetToken;
using CarShop.Application.Features.Auth.Commands.Login;
using CarShop.Application.Features.Auth.Commands.Logout;
using CarShop.Application.Features.Auth.Commands.Register;
using CarShop.Application.Features.Auth.Commands.ResetPassword;
using CarShop.Application.Features.Auth.Queries.EmailExists;
using CarShop.Application.Features.User.Commands.ChangePassword;
using CarShop.Application.Features.User.Commands.UpdateProfile;
using CarShop.Application.Features.User.Queries.GetProfile;
using CarShop.Application.Interfaces;
using CarShop.Infrastructure.Identity;
using CarShop.Web.ViewModels.Account;
using CarShop.Web.ViewModels.Mappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace CarShop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IUserContextService _userContextService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            IMediator mediator,
            IUserContextService userContextService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userContextService = userContextService;
            _signInManager = signInManager;
            _userManager = userManager;
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
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = AccountMapper.ToDto(model);
            var result = await _mediator.Send(new RegisterCommand(dto));

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
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            var dto = AccountMapper.ToDto(model);
            var result = await _mediator.Send(new LoginCommand(dto));

            if (!result.Success)
            {
                if (result.FieldErrors != null)
                {
                    foreach (var kvp in result.FieldErrors)
                        ModelState.AddModelError(kvp.Key, kvp.Value);
                }
                else if (result.Errors != null && result.Errors.Any())
                {
                    TempData["ErrorMessage"] = result.Errors.First();
                }

                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            return RedirectToLocal(returnUrl);
        }


        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var result = await _mediator.Send(new LogoutCommand());
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> CheckEmailExists(string email)
        {
            var result = await _mediator.Send(new EmailExistsQuery(email));
            return Json(result.Data);
        }


        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var result = await _mediator.Send(new GetProfileQuery());

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

            var result = await _mediator.Send(new UpdateProfileCommand(dto));

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message ?? "Profile update failed.";
                return View("Profile", model);
            }

            TempData["SuccessMessage"] = result.Message ?? "Profile updated.";
            return RedirectToAction("Profile");
        }


        [Authorize]
        public async Task<IActionResult> ChangePassword()
        {
            var profile = await _mediator.Send(new GetProfileQuery());
            ViewBag.FullName = profile.Data?.FullName;
            ViewBag.Email    = profile.Data?.Email;
            return View();
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var profile = await _mediator.Send(new GetProfileQuery());
                ViewBag.FullName = profile.Data?.FullName;
                ViewBag.Email    = profile.Data?.Email;
                return View(model);
            }

            var dto = AccountMapper.ToDto(model);
            var result = await _mediator.Send(new ChangePasswordCommand(dto));

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
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = AccountMapper.ToDto(model);
            var result = await _mediator.Send(new GeneratePasswordResetTokenCommand(dto.Email!));

            if (!result.Success)
            {
                TempData["ErrorMessage"] = string.Join(", ", result.Errors!);
                return View(model);
            }

            var token = result.Data;
            var resetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token }, Request.Scheme);

            // TODO: replace with real email sending
            // For development: log or display link; in production send via IEmailService
            // await _emailService.SendEmailAsync(model.Email, "Reset Password", $"<a href='{resetLink}'>Click here</a>");

            ViewBag.EmailSent = true;
            return View(model);
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
            var result = await _mediator.Send(new ResetPasswordCommand(dto.Email!, dto.Token!, dto.NewPassword!));

            if (!result.Success)
            {
                TempData["ErrorMessage"] = string.Join("; ", result.Errors!);
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });
            var properties  = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl!);
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["ErrorMessage"] = "Google sign-in failed. Please try again.";
                return RedirectToAction("Login");
            }

            // 1. Existing linked account → sign in directly
            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
                return RedirectToLocal(returnUrl);

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Could not retrieve email from Google.";
                return RedirectToAction("Login");
            }

            // 2. Account with same email exists → link Google and sign in
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                await _userManager.AddLoginAsync(existingUser,
                    new UserLoginInfo(info.LoginProvider, info.ProviderKey, info.ProviderDisplayName));
                await _signInManager.SignInAsync(existingUser, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }

            // 3. No account at all → auto-register
            var fullName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email;
            var result   = await _mediator.Send(new ExternalRegisterAndSignInCommand(email, fullName, info.LoginProvider, info.ProviderKey));

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message ?? "Could not sign in with Google.";
                return RedirectToAction("Login");
            }

            return RedirectToLocal(returnUrl);
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
