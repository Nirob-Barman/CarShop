using System.Text.Json;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using MediatR;
using PaymentGatewayEntity = CarShop.Domain.Entities.PaymentGateway;

namespace CarShop.Application.Features.PaymentGateway.Queries.GetDecryptedGatewayConfig
{
    public class GetDecryptedGatewayConfigQueryHandler : IRequestHandler<GetDecryptedGatewayConfigQuery, Dictionary<string, string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfigEncryptor _encryptor;

        public GetDecryptedGatewayConfigQueryHandler(IUnitOfWork unitOfWork, IConfigEncryptor encryptor)
        {
            _unitOfWork = unitOfWork;
            _encryptor = encryptor;
        }

        public async Task<Dictionary<string, string>> Handle(GetDecryptedGatewayConfigQuery request, CancellationToken cancellationToken)
        {
            var gateway = await _unitOfWork.Repository<PaymentGatewayEntity>().GetByIdAsync(request.Id);
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
