using CarShop.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BulkImportController : Controller
    {
        private readonly IBulkImportService _bulkImportService;

        public BulkImportController(IBulkImportService bulkImportService)
        {
            _bulkImportService = bulkImportService;
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
            var result = await _bulkImportService.ImportCarsFromCsvAsync(stream);

            ViewBag.ImportResult = result.Data;
            TempData["SuccessMessage"] = result.Message;
            return View();
        }
    }
}
