using CarShop.Application.DTOs.Payment;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Mappers;
using CarShop.Application.Wrappers;
using MediatR;
using PaymentGatewayEntity = CarShop.Domain.Entities.PaymentGateway;

namespace CarShop.Application.Features.PaymentGateway.Queries.GetGatewayById
{
    public class GetGatewayByIdQueryHandler : IRequestHandler<GetGatewayByIdQuery, Result<PaymentGatewayDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetGatewayByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PaymentGatewayDto>> Handle(GetGatewayByIdQuery request, CancellationToken cancellationToken)
        {
            var gateway = await _unitOfWork.Repository<PaymentGatewayEntity>().GetByIdAsync(request.Id);
            if (gateway == null) return Result<PaymentGatewayDto>.Fail("Gateway not found.");
            return Result<PaymentGatewayDto>.Ok(PaymentGatewayMapper.ToDto(gateway));
        }
    }
}
