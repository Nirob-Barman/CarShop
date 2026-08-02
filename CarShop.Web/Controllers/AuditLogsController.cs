using CarShop.Application.Features.AuditLog.Queries.GetAuditLogs;
using CarShop.Application.Features.AuditLog.Queries.GetDistinctAuditLogEntityNames;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuditLogsController : Controller
    {
        private readonly IMediator _mediator;

        public AuditLogsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(string? entityName, int page = 1)
        {
            var result = await _mediator.Send(new GetAuditLogsQuery(entityName, page, 50));
            ViewBag.EntityName = entityName;
            ViewBag.CurrentPage = page;
            ViewBag.EntityNames = await _mediator.Send(new GetDistinctAuditLogEntityNamesQuery());
            return View(result.Data ?? Enumerable.Empty<CarShop.Application.DTOs.AuditLog.AuditLogDto>());
        }
    }
}
