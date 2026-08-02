using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using OrderEntity = CarShop.Domain.Entities.Order;

namespace CarShop.Application.Features.Order.Queries.GetCompletedOrdersCount
{
    public class GetCompletedOrdersCountQueryHandler : IRequestHandler<GetCompletedOrdersCountQuery, Result<int>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCompletedOrdersCountQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(GetCompletedOrdersCountQuery request, CancellationToken cancellationToken)
        {
            var count = await _unitOfWork.Repository<OrderEntity>()
                .CountAsync(o => o.Status == "Confirmed" || o.Status == "Paid");
            return Result<int>.Ok(count);
        }
    }
}
