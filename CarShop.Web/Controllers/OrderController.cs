using CarShop.Application.Features.Order.Commands.CancelOrder;
using CarShop.Application.Features.Order.Commands.ExpireStalePendingOrders;
using CarShop.Application.Features.Order.Queries.GetOrderById;
using CarShop.Application.Features.Order.Queries.GetOrdersByUserId;
using CarShop.Application.Interfaces;
using CarShop.Web.ViewModels.Mappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize]
    public class OrderController : UserDashboardController
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator, IUserContextService userContextService)
            : base(mediator, userContextService)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            try
            {
                await _mediator.Send(new CancelOrderCommand(orderId));
                TempData["SuccessMessage"] = "Order canceled successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("MyOrders");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _mediator.Send(new GetOrderByIdQuery(id));
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("MyOrders");
            }
            return View(OrderMapper.ToViewModel(result.Data!));
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            await _mediator.Send(new ExpireStalePendingOrdersCommand());
            var orders = await _mediator.Send(new GetOrdersByUserIdQuery());
            var ordersVm = OrderMapper.ToViewModels(orders.Data!);
            return View(ordersVm);
        }
    }
}
