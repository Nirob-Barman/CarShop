using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Order.Commands.MarkOrderAsPaid
{
    public record MarkOrderAsPaidCommand(int OrderId)
        : IRequest<Result<string>>;
}
