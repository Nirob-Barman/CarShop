using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Payment.Commands.HandlePaymentCancel
{
    public record HandlePaymentCancelCommand(int TransactionDbId)
        : IRequest<Result<string>>;
}
