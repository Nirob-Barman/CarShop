using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Commands.SetOrderGateway
{
    public class SetOrderGatewayCommandHandler : IRequestHandler<SetOrderGatewayCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;

        public SetOrderGatewayCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<string>> Handle(SetOrderGatewayCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FindAsync(request.OrderId);
            if (order == null) return Result<string>.Ok(null, "Order not found.");

            order.PaymentGatewayId = request.PaymentGatewayId;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "Gateway set.");
        }
    }
}
