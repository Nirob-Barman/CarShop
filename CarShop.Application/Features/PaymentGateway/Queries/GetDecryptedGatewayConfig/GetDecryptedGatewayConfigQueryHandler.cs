using System.Text.Json;
using CarShop.Application.Interfaces;
using MediatR;

namespace CarShop.Application.Features.PaymentGateway.Queries.GetDecryptedGatewayConfig
{
    public class GetDecryptedGatewayConfigQueryHandler : IRequestHandler<GetDecryptedGatewayConfigQuery, Dictionary<string, string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IConfigEncryptor _encryptor;

        public GetDecryptedGatewayConfigQueryHandler(IApplicationDbContext context, IConfigEncryptor encryptor)
        {
            _context = context;
            _encryptor = encryptor;
        }

        public async Task<Dictionary<string, string>> Handle(GetDecryptedGatewayConfigQuery request, CancellationToken cancellationToken)
        {
            var gateway = await _context.PaymentGateways.FindAsync(request.Id);
            if (gateway == null) return [];

            Dictionary<string, string> result = [];
            if (gateway.Config != null)
            {
                try
                {
                    var json = _encryptor.Decrypt(gateway.Config);
                    result = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? [];
                }
                catch { }
            }

            // Always inject sandbox flag so processors can use it without a separate DB call
            result["_is_sandbox"] = gateway.IsSandbox ? "true" : "false";
            return result;
        }
    }
}
