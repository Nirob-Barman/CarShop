using CarShop.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    /// <summary>
    /// Base controller for user dashboard pages.
    /// Automatically sets ViewBag.UserFullName for the _UserLayout sidebar.
    /// </summary>
    public abstract class UserDashboardController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserContextService _userContextService;

        protected UserDashboardController(IUserService userService, IUserContextService userContextService)
        {
            _userService = userService;
            _userContextService = userContextService;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (_userContextService.IsAuthenticated && !string.IsNullOrEmpty(_userContextService.UserId))
            {
                var profile = await _userService.GetProfileAsync(_userContextService.UserId);
                ViewBag.UserFullName = profile.Data?.FullName;
            }

            await next();
        }
    }
}
