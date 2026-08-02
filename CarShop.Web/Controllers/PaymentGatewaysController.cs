using CarShop.Application.Features.PaymentGateway.Commands.CreateGateway;
using CarShop.Application.Features.PaymentGateway.Commands.DeleteGateway;
using CarShop.Application.Features.PaymentGateway.Commands.ToggleGatewayActive;
using CarShop.Application.Features.PaymentGateway.Commands.UpdateGateway;
using CarShop.Application.Features.PaymentGateway.Queries.GetAllGateways;
using CarShop.Application.Features.PaymentGateway.Queries.GetDecryptedGatewayConfig;
using CarShop.Application.Features.PaymentGateway.Queries.GetGatewayById;
using CarShop.Application.Payment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PaymentGatewaysController : Controller
    {
        private readonly IMediator _mediator;

        public PaymentGatewaysController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _mediator.Send(new GetAllGatewaysQuery());
            return View(result.Data ?? []);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(
            string name, string slug, string? gatewayFamily, string type, string? logoUrl,
            bool isActive, bool isSandbox, string supportedCurrencies, int sortOrder,
            [FromForm] Dictionary<string, string> configFields)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                TempData["ErrorMessage"] = "Please select a gateway service type.";
                return RedirectToAction("Add");
            }

            var dto = new CarShop.Application.DTOs.Payment.PaymentGatewayDto
            {
                Name = name,
                Slug = slug,
                GatewayFamily = string.IsNullOrWhiteSpace(gatewayFamily) ? GatewayConfigSchema.GetFamilyKey(slug) : gatewayFamily,
                Type = type,
                LogoUrl = logoUrl,
                IsActive = isActive, IsSandbox = isSandbox,
                SupportedCurrencies = supportedCurrencies, SortOrder = sortOrder
            };

            var result = await _mediator.Send(new CreateGatewayCommand(dto, configFields));
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _mediator.Send(new GetGatewayByIdQuery(id));
            if (!result.Success) return RedirectToAction("Index");
            var config = await _mediator.Send(new GetDecryptedGatewayConfigQuery(id));
            ViewBag.Config = config;
            ViewBag.Schema = GatewayConfigSchema.Get(result.Data!.Slug);
            ViewBag.Family = GatewayConfigSchema.GetFamily(result.Data!.Slug);
            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, string name, string? gatewayFamily, string type, string? logoUrl,
            bool isActive, bool isSandbox, string supportedCurrencies, int sortOrder,
            [FromForm] Dictionary<string, string> configFields)
        {
            var dto = new CarShop.Application.DTOs.Payment.PaymentGatewayDto
            {
                Name = name,
                GatewayFamily = gatewayFamily ?? string.Empty,
                Type = type,
                LogoUrl = logoUrl,
                IsActive = isActive, IsSandbox = isSandbox,
                SupportedCurrencies = supportedCurrencies, SortOrder = sortOrder
            };

            var newConfig = configFields.Any(kv => !string.IsNullOrWhiteSpace(kv.Value)) ? configFields : null;
            var result = await _mediator.Send(new UpdateGatewayCommand(id, dto, newConfig));
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            await _mediator.Send(new ToggleGatewayActiveCommand(id));
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteGatewayCommand(id));
            if (result.Success) TempData["SuccessMessage"] = result.Message;
            else TempData["ErrorMessage"] = result.Errors?.FirstOrDefault();
            return RedirectToAction("Index");
        }
    }
}
