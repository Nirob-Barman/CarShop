﻿using System.Security.Claims;
using CarShop.Application.DTOs.Identity;
using CarShop.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Register()
        {
            if (User.Identity!.IsAuthenticated)
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

            //bool emailExists = await _userService.EmailExistsAsync(model.Email!);
            //if (emailExists)
            //{
            //    ModelState.AddModelError(nameof(model.Email), "This email is already registered.");
            //    return View(model);
            //}

            try
            {                
                await _userService.RegisterAsync(model);
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity!.IsAuthenticated)
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

            var existingUser = await _userService.GetUserByEmailAsync(model.Email!);
            if (existingUser is null)
            {
                ViewData["ReturnUrl"] = returnUrl;
                ModelState.AddModelError(nameof(model.Email), "This email is not registered.");
                return View(model);
            }

            var isPasswordMatch = await _userService.CheckPasswordAsync(existingUser, model.Password!);
            if (!isPasswordMatch)
            {
                ModelState.AddModelError(nameof(model.Password), "Incorrect password.");
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            try
            {
                await _userService.LoginAsync(model);
                //return RedirectToAction("Index", "Home");
                return RedirectToLocal(returnUrl);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();
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
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var profile = await _userService.GetProfileAsync(userId);
            return View(profile);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileDto model)
        {
            if (!ModelState.IsValid)
                return View("Profile", model);

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            try
            {
                await _userService.UpdateProfileAsync(userId, model);
                TempData["SuccessMessage"] = "Profile Updated successfully.";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                //return View(model);
                return View("Profile", model);
            }
        }

        [Authorize]
        public IActionResult ChangePassword() => View();

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            try
            {
                await _userService.ChangePasswordAsync(userId, model);
                TempData["SuccessMessage"] = "Password changed successfully.";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var token = await _userService.GeneratePasswordResetTokenAsync(model.Email!);
                //It generates a full absolute URL(including protocol and domain) that points to the ResetPassword action on your AccountController, with query parameters for the email and token.
                //Request.Scheme Ensures the generated link uses http:// or https:// depending on the request. This makes the URL absolute.
                var resetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token }, Request.Scheme);

                // TODO: Send resetLink via email (simulated below)
                TempData["ResetLink"] = resetLink; // For dev/demo purpose
                ViewBag.Message = "Reset link generated. (Check email)";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

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

            try
            {
                await _userService.ResetPasswordAsync(model.Email!, model.Token!, model.NewPassword!);
                TempData["SuccessMessage"] = "Password has been reset successfully.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
            }
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
