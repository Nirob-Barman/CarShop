using CarShop.Application.Features.Order.Commands.CancelPendingOrderById;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using PaymentTransactionEntity = CarShop.Domain.Entities.PaymentTransaction;

namespace CarShop.Application.Features.Payment.Commands.HandlePaymentCancel
{
    public class HandlePaymentCancelCommandHandler : IRequestHandler<HandlePaymentCancelCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public HandlePaymentCancelCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<Result<string>> Handle(HandlePaymentCancelCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _unitOfWork.Repository<PaymentTransactionEntity>()
                .GetByIdAsync(request.TransactionDbId);

            if (transaction == null || transaction.Status != "Pending")
                return Result<string>.Ok(null, "Nothing to cancel.");

            transaction.Status = "Failed";
            _unitOfWork.Repository<PaymentTransactionEntity>().Update(transaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _mediator.Send(new CancelPendingOrderByIdCommand(transaction.OrderId), cancellationToken);
            return Result<string>.Ok(null, "Payment cancelled.");
        }
    }
}
