using MediatR;

namespace CarShop.Application.Features.PaymentGateway.Queries.GetDecryptedGatewayConfig
{
    public class GetDecryptedGatewayConfigQuery : IRequest<Dictionary<string, string>>
    {
        public int Id { get; set; }

        public GetDecryptedGatewayConfigQuery(int id)
        {
            Id = id;
        }
    }
}
