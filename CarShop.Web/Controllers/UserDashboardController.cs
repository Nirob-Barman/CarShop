using CarShop.Application.Features.User.Queries.GetProfile;
using CarShop.Application.Interfaces;
using MediatR;
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
        private readonly IMediator _mediator;
        private readonly IUserContextService _userContextService;

        protected UserDashboardController(IMediator mediator, IUserContextService userContextService)
        {
            _mediator = mediator;
            _userContextService = userContextService;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (_userContextService.IsAuthenticated && !string.IsNullOrEmpty(_userContextService.UserId))
            {
                var profile = await _mediator.Send(new GetProfileQuery());
                ViewBag.UserFullName = profile.Data?.FullName;
            }

            await next();
        }
    }
}
