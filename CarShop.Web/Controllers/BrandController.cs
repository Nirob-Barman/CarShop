using CarShop.Application.Features.Brand.Commands.CreateBrand;
using CarShop.Application.Features.Brand.Commands.DeleteBrand;
using CarShop.Application.Features.Brand.Commands.UpdateBrand;
using CarShop.Application.Features.Brand.Queries.GetAllBrands;
using CarShop.Application.Features.Brand.Queries.GetBrandById;
using CarShop.Web.ViewModels.Brand;
using CarShop.Web.ViewModels.Mappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BrandController : Controller
    {
        private readonly IMediator _mediator;

        public BrandController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _mediator.Send(new GetAllBrandsQuery());
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message ?? "Failed to load brands.";
                return View(new List<BrandViewModel>());
            }
            return View(BrandMapper.ToViewModels(result.Data!));
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _mediator.Send(new CreateBrandCommand(model.Name));
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message ?? result.Errors?.FirstOrDefault();
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _mediator.Send(new GetBrandByIdQuery(id));
            if (!result.Success || result.Data == null)
            {
                TempData["ErrorMessage"] = result.Message ?? "Brand not found.";
                return RedirectToAction("Index");
            }
            return View(BrandMapper.ToViewModel(result.Data!));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BrandViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _mediator.Send(new UpdateBrandCommand(id, model.Name));
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message ?? result.Errors?.FirstOrDefault();
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteBrandCommand(id));
            if (!result.Success)
                TempData["ErrorMessage"] = result.Message ?? "Failed to delete brand.";
            else
                TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }
    }
}
