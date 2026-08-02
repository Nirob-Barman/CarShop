using CarShop.Application.Features.BulkImport.Commands.ImportCarsFromCsv;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BulkImportController : Controller
    {
        private readonly IMediator _mediator;

        public BulkImportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a CSV file.";
                return View();
            }

            using var stream = csvFile.OpenReadStream();
            var result = await _mediator.Send(new ImportCarsFromCsvCommand(stream));

            ViewBag.ImportResult = result.Data;
            TempData["SuccessMessage"] = result.Message;
            return View();
        }
    }
}
