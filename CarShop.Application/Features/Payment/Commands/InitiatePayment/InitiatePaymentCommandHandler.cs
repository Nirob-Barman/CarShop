using CarShop.Application.Features.Order.Commands.CancelPendingOrderById;
using CarShop.Application.Features.Order.Commands.CreatePendingOrder;
using CarShop.Application.Features.Order.Commands.SetOrderGateway;
using CarShop.Application.Features.PaymentGateway.Queries.GetDecryptedGatewayConfig;
using CarShop.Application.Features.PaymentGateway.Queries.GetGatewayById;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using MediatR;
using PaymentTransactionEntity = CarShop.Domain.Entities.PaymentTransaction;

namespace CarShop.Application.Features.Payment.Commands.InitiatePayment
{
    public class InitiatePaymentCommandHandler : IRequestHandler<InitiatePaymentCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentProcessorFactory _processorFactory;
        private readonly IMediator _mediator;

        public InitiatePaymentCommandHandler(
            IUnitOfWork unitOfWork,
            IPaymentProcessorFactory processorFactory,
            IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _processorFactory = processorFactory;
            _mediator = mediator;
        }

        public async Task<Result<string>> Handle(InitiatePaymentCommand request, CancellationToken cancellationToken)
        {
            var gatewayResult = await _mediator.Send(new GetGatewayByIdQuery(request.GatewayId), cancellationToken);
            if (!gatewayResult.Success || gatewayResult.Data == null)
                return Result<string>.Fail("Invalid payment gateway.");

            var gateway = gatewayResult.Data;
            if (!gateway.IsActive)
                return Result<string>.Fail("Selected payment gateway is not available.");

            if (!_processorFactory.HasProcessor(gateway.Slug))
                return Result<string>.Fail($"Payment processor for '{gateway.Name}' is not configured.");

            var orderResult = await _mediator.Send(new CreatePendingOrderCommand(request.CarId, request.PromoCode), cancellationToken);
            if (!orderResult.Success)
                return Result<string>.Fail(orderResult.Errors?.FirstOrDefault() ?? "Could not create order.");

            var (orderId, finalPrice, carTitle) = orderResult.Data;

            await _mediator.Send(new SetOrderGatewayCommand(orderId, request.GatewayId), cancellationToken);

            var transaction = new PaymentTransactionEntity
            {
                OrderId          = orderId,
                PaymentGatewayId = request.GatewayId,
                Amount           = finalPrice,
                Currency         = "USD",
                CreatedAt        = DateTime.UtcNow
            };
            await _unitOfWork.Repository<PaymentTransactionEntity>().AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var config = await _mediator.Send(new GetDecryptedGatewayConfigQuery(request.GatewayId), cancellationToken);
            var processor = _processorFactory.GetProcessor(gateway.Slug);

            var fullSuccessUrl = $"{request.SuccessUrl}?txId={transaction.Id}&gateway={gateway.Slug}";
            var fullCancelUrl  = $"{request.CancelUrl}?txId={transaction.Id}";

            var paymentRequest = new PaymentRequest(
                orderId, transaction.Id, carTitle, finalPrice, "USD",
                fullSuccessUrl, fullCancelUrl, config);

            var initResult = await processor.InitiateAsync(paymentRequest);
            if (!initResult.Success)
            {
                await _mediator.Send(new CancelPendingOrderByIdCommand(orderId), cancellationToken);
                transaction.MarkFailed();
                _unitOfWork.Repository<PaymentTransactionEntity>().Update(transaction);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<string>.Fail(initResult.Error ?? "Payment initiation failed.");
            }

            transaction.SessionRef = initResult.SessionRef;
            _unitOfWork.Repository<PaymentTransactionEntity>().Update(transaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(initResult.RedirectUrl!);
        }
    }
}
