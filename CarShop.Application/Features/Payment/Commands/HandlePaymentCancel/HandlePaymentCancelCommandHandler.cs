using CarShop.Application.Interfaces;
using CarShop.Application.Features.Order.Commands.CancelPendingOrderById;
using CarShop.Application.Wrappers;
using CarShop.Domain.Enums;
using MediatR;

namespace CarShop.Application.Features.Payment.Commands.HandlePaymentCancel
{
    public class HandlePaymentCancelCommandHandler : IRequestHandler<HandlePaymentCancelCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public HandlePaymentCancelCommandHandler(IApplicationDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<Result<string>> Handle(HandlePaymentCancelCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _context.PaymentTransactions
                .FindAsync(request.TransactionDbId);

            if (transaction == null || transaction.Status != PaymentTransactionStatus.Pending)
                return Result<string>.Ok(null, "Nothing to cancel.");

            transaction.MarkFailed();
            _context.PaymentTransactions.Update(transaction);
            await _context.SaveChangesAsync(cancellationToken);

            await _mediator.Send(new CancelPendingOrderByIdCommand(transaction.OrderId), cancellationToken);
            return Result<string>.Ok(null, "Payment cancelled.");
        }
    }
}
