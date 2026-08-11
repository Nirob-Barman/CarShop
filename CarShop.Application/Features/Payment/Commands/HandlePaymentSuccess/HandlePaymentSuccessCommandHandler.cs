using CarShop.Application.Features.Notification.Commands.CreateNotification;
using CarShop.Application.Features.Order.Commands.MarkOrderAsPaid;
using CarShop.Application.Features.PaymentGateway.Queries.GetDecryptedGatewayConfig;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using CarShop.Domain.Enums;
using MediatR;
using CarEntity = CarShop.Domain.Entities.Car;
using OrderEntity = CarShop.Domain.Entities.Order;
using PaymentTransactionEntity = CarShop.Domain.Entities.PaymentTransaction;

namespace CarShop.Application.Features.Payment.Commands.HandlePaymentSuccess
{
    public class HandlePaymentSuccessCommandHandler : IRequestHandler<HandlePaymentSuccessCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentProcessorFactory _processorFactory;
        private readonly IMediator _mediator;
        private readonly IEmailService _emailService;
        private readonly IUserManager _userManager;

        public HandlePaymentSuccessCommandHandler(
            IUnitOfWork unitOfWork,
            IPaymentProcessorFactory processorFactory,
            IMediator mediator,
            IEmailService emailService,
            IUserManager userManager)
        {
            _unitOfWork = unitOfWork;
            _processorFactory = processorFactory;
            _mediator = mediator;
            _emailService = emailService;
            _userManager = userManager;
        }

        public async Task<Result<string>> Handle(HandlePaymentSuccessCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _unitOfWork.Repository<PaymentTransactionEntity>()
                .GetByIdAsync(request.TransactionDbId);

            if (transaction == null)
                return Result<string>.Fail("Transaction not found.");

            if (transaction.Status == PaymentTransactionStatus.Success)
                return Result<string>.Ok(null, "Already confirmed.");

            var config = await _mediator.Send(new GetDecryptedGatewayConfigQuery(transaction.PaymentGatewayId), cancellationToken);
            var processor = _processorFactory.GetProcessor(request.GatewaySlug);

            var verifyResult = await processor.VerifyAsync(request.SessionRefOverride ?? transaction.SessionRef ?? request.TransactionDbId.ToString(), config);

            transaction.RecordVerificationResult(verifyResult.Success, verifyResult.ProviderTransactionId, verifyResult.RawResponse);
            _unitOfWork.Repository<PaymentTransactionEntity>().Update(transaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!verifyResult.Success)
                return Result<string>.Fail("Payment verification failed.");

            await _mediator.Send(new MarkOrderAsPaidCommand(transaction.OrderId), cancellationToken);

            var order = await _unitOfWork.Repository<OrderEntity>().GetByIdAsync(transaction.OrderId);
            var car   = order != null ? await _unitOfWork.Repository<CarEntity>().GetByIdAsync(order.CarId) : null;

            if (order?.UserId != null)
            {
                await _mediator.Send(new CreateNotificationCommand(order.UserId,
                    $"Payment confirmed for {car?.Title ?? "your car"}!",
                    "/Order/MyOrders"), cancellationToken);

                try
                {
                    var user = await _userManager.FindByIdAsync(order.UserId);
                    if (user?.Email != null)
                        await _emailService.SendEmailAsync(user.Email,
                            "Order Confirmed - CarShop",
                            $"<h2>Payment Successful!</h2><p>Your order for <strong>{car?.Title}</strong> is confirmed.</p><p>Total: <strong>${order.FinalPrice:F2}</strong></p>");
                }
                catch { }
            }

            return Result<string>.Ok(null, "Payment confirmed.");
        }
    }
}
