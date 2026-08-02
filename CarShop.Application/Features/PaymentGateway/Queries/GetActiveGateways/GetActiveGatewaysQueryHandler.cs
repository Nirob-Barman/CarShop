using CarShop.Application.DTOs.Payment;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Mappers;
using CarShop.Application.Wrappers;
using MediatR;
using PaymentGatewayEntity = CarShop.Domain.Entities.PaymentGateway;

namespace CarShop.Application.Features.PaymentGateway.Queries.GetActiveGateways
{
    public class GetActiveGatewaysQueryHandler : IRequestHandler<GetActiveGatewaysQuery, Result<IEnumerable<PaymentGatewayDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetActiveGatewaysQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<PaymentGatewayDto>>> Handle(GetActiveGatewaysQuery request, CancellationToken cancellationToken)
        {
            var gateways = await _unitOfWork.Repository<PaymentGatewayEntity>()
                .GetAllAsync(g => g.IsActive, g => PaymentGatewayMapper.ToDto(g));
            return Result<IEnumerable<PaymentGatewayDto>>.Ok(gateways.OrderBy(g => g.SortOrder));
        }
    }
}
