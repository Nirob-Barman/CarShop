using CarShop.Application.Features.Analytics.Queries.GetDashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Dashboard()
        {
            var result = await _mediator.Send(new GetDashboardQuery());
            return View(result.Data);
        }
    }
}
