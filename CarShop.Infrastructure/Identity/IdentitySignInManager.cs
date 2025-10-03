using CarShop.Application.Interfaces.Identity;
using CarShop.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CarShop.Infrastructure.Identity
{
    public class IdentitySignInManager : ISignInManager
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IdentitySignInManager(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<bool> CheckPasswordSignInAsync(AppUser user, string password)
        {
            var identityUser = await _signInManager.UserManager.FindByIdAsync(user.Id!.ToString());
            if (identityUser == null) return false;

            var result = await _signInManager.CheckPasswordSignInAsync(identityUser, password, false);
            return result.Succeeded;
        }

        public async Task SignInAsync(AppUser user, bool isPersistent)
        {
            var identityUser = await _signInManager.UserManager.FindByIdAsync(user.Id!.ToString());
            if (identityUser != null)
            {
                await _signInManager.SignInAsync(identityUser, isPersistent);
            }
        }


        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task RefreshSignInAsync(AppUser user)
        {
            var identityUser = await _signInManager.UserManager.FindByIdAsync(user.Id!.ToString());
            if (identityUser != null)
            {
                await _signInManager.RefreshSignInAsync(identityUser);
            }
        }
    }
}