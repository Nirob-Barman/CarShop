using CarShop.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuditLogsController : Controller
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogsController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        public async Task<IActionResult> Index(string? entityName, int page = 1)
        {
            var result = await _auditLogService.GetLogsAsync(entityName, page, 50);
            ViewBag.EntityName = entityName;
            ViewBag.CurrentPage = page;
            ViewBag.EntityNames = await _auditLogService.GetDistinctEntityNamesAsync();
            return View(result.Data ?? Enumerable.Empty<CarShop.Application.DTOs.AuditLog.AuditLogDto>());
        }
    }
}
