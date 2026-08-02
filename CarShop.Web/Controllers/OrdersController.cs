using CarShop.Application.Features.Order.Commands.AdminCancelOrder;
using CarShop.Application.Features.Order.Queries.GetAllOrders;
using CarShop.Application.Features.Order.Queries.GetOrderByIdAdmin;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(string? status, int page = 1)
        {
            var result = await _mediator.Send(new GetAllOrdersQuery(status, page, 20));
            ViewBag.Status = status;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = result.Data?.TotalPages ?? 1;
            return View(result.Data?.Items ?? Enumerable.Empty<CarShop.Application.DTOs.Order.OrderDto>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await _mediator.Send(new GetOrderByIdAdminQuery(id));
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Index");
            }
            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int orderId)
        {
            var result = await _mediator.Send(new AdminCancelOrderCommand(orderId));
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "Could not cancel order.";
            return RedirectToAction("Index");
        }
    }
}
