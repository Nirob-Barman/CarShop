using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using OrderEntity = CarShop.Domain.Entities.Order;

namespace CarShop.Application.Features.Order.Commands.SetOrderGateway
{
    public class SetOrderGatewayCommandHandler : IRequestHandler<SetOrderGatewayCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SetOrderGatewayCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(SetOrderGatewayCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Repository<OrderEntity>().GetByIdAsync(request.OrderId);
            if (order == null) return Result<string>.Ok(null, "Order not found.");

            order.PaymentGatewayId = request.PaymentGatewayId;
            _unitOfWork.Repository<OrderEntity>().Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "Gateway set.");
        }
    }
}
