using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Payment.Commands.HandlePaymentCancel
{
    public class HandlePaymentCancelCommand : IRequest<Result<string>>
    {
        public int TransactionDbId { get; set; }

        public HandlePaymentCancelCommand(int transactionDbId)
        {
            TransactionDbId = transactionDbId;
        }
    }
}
