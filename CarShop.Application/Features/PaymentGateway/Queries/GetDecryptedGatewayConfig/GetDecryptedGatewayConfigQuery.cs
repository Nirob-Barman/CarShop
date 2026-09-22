using MediatR;

namespace CarShop.Application.Features.PaymentGateway.Queries.GetDecryptedGatewayConfig
{
    public record GetDecryptedGatewayConfigQuery(int Id)
        : IRequest<Dictionary<string, string>>;
}
