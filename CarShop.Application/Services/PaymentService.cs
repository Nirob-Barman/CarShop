using CarShop.Application.DTOs.Payment;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;

namespace CarShop.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork              _unitOfWork;
        private readonly IOrderService            _orderService;
        private readonly IPaymentGatewayService   _gatewayService;
        private readonly IPaymentProcessorFactory _processorFactory;
        private readonly INotificationService     _notificationService;
        private readonly IEmailService            _emailService;
        private readonly IUserManager             _userManager;
        private readonly IUserContextService      _userContextService;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IOrderService orderService,
            IPaymentGatewayService gatewayService,
            IPaymentProcessorFactory processorFactory,
            INotificationService notificationService,
            IEmailService emailService,
            IUserManager userManager,
            IUserContextService userContextService)
        {
            _unitOfWork          = unitOfWork;
            _orderService        = orderService;
            _gatewayService      = gatewayService;
            _processorFactory    = processorFactory;
            _notificationService = notificationService;
            _emailService        = emailService;
            _userManager         = userManager;
            _userContextService  = userContextService;
        }

        public async Task<Result<string>> InitiatePaymentAsync(
            int carId, int gatewayId, string? promoCode,
            string successUrl, string cancelUrl)
        {
            var userId = _userContextService.UserId!;
            var gatewayResult = await _gatewayService.GetByIdAsync(gatewayId);
            if (!gatewayResult.Success || gatewayResult.Data == null)
                return Result<string>.Fail("Invalid payment gateway.");

            var gateway = gatewayResult.Data;
            if (!gateway.IsActive)
                return Result<string>.Fail("Selected payment gateway is not available.");

            if (!_processorFactory.HasProcessor(gateway.Slug))
                return Result<string>.Fail($"Payment processor for '{gateway.Name}' is not configured.");

            var orderResult = await _orderService.CreatePendingOrderAsync(carId, promoCode);
            if (!orderResult.Success)
                return Result<string>.Fail(orderResult.Errors?.FirstOrDefault() ?? "Could not create order.");

            var (orderId, finalPrice, carTitle) = orderResult.Data;

            await _orderService.SetOrderGatewayAsync(orderId, gatewayId);

            var transaction = new PaymentTransaction
            {
                OrderId          = orderId,
                PaymentGatewayId = gatewayId,
                Amount           = finalPrice,
                Currency         = "USD",
                Status           = "Pending",
                CreatedAt        = DateTime.UtcNow
            };
            await _unitOfWork.Repository<PaymentTransaction>().AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            var config = await _gatewayService.GetDecryptedConfigAsync(gatewayId);
            var processor = _processorFactory.GetProcessor(gateway.Slug);

            var fullSuccessUrl = $"{successUrl}?txId={transaction.Id}&gateway={gateway.Slug}";
            var fullCancelUrl  = $"{cancelUrl}?txId={transaction.Id}";

            var request = new PaymentRequest(
                orderId, transaction.Id, carTitle, finalPrice, "USD",
                fullSuccessUrl, fullCancelUrl, config);

            var initResult = await processor.InitiateAsync(request);
            if (!initResult.Success)
            {
                await _orderService.CancelPendingOrderByIdAsync(orderId);
                transaction.Status = "Failed";
                _unitOfWork.Repository<PaymentTransaction>().Update(transaction);
                await _unitOfWork.SaveChangesAsync();
                return Result<string>.Fail(initResult.Error ?? "Payment initiation failed.");
            }

            transaction.SessionRef = initResult.SessionRef;
            _unitOfWork.Repository<PaymentTransaction>().Update(transaction);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Ok(initResult.RedirectUrl!);
        }

        public async Task<Result<string>> HandleSuccessAsync(int transactionDbId, string gatewaySlug, string? sessionRefOverride = null)
        {
            var transaction = await _unitOfWork.Repository<PaymentTransaction>()
                .GetByIdAsync(transactionDbId);

            if (transaction == null)
                return Result<string>.Fail("Transaction not found.");

            if (transaction.Status == "Success")
                return Result<string>.Ok(null, "Already confirmed.");

            var config = await _gatewayService.GetDecryptedConfigAsync(transaction.PaymentGatewayId);
            var processor = _processorFactory.GetProcessor(gatewaySlug);

            var verifyResult = await processor.VerifyAsync(sessionRefOverride ?? transaction.SessionRef ?? transactionDbId.ToString(), config);

            transaction.Status        = verifyResult.Success ? "Success" : "Failed";
            transaction.TransactionId = verifyResult.ProviderTransactionId;
            transaction.RawResponse   = verifyResult.RawResponse;
            _unitOfWork.Repository<PaymentTransaction>().Update(transaction);
            await _unitOfWork.SaveChangesAsync();

            if (!verifyResult.Success)
                return Result<string>.Fail("Payment verification failed.");

            await _orderService.MarkOrderAsPaidAsync(transaction.OrderId);

            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(transaction.OrderId);
            var car   = order != null ? await _unitOfWork.Repository<Car>().GetByIdAsync(order.CarId) : null;

            if (order?.UserId != null)
            {
                await _notificationService.CreateNotificationAsync(order.UserId,
                    $"Payment confirmed for {car?.Title ?? "your car"}!",
                    "/Order/MyOrders");

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

        public async Task<Result<string>> HandleCancelAsync(int transactionDbId)
        {
            var transaction = await _unitOfWork.Repository<PaymentTransaction>()
                .GetByIdAsync(transactionDbId);

            if (transaction == null || transaction.Status != "Pending")
                return Result<string>.Ok(null, "Nothing to cancel.");

            transaction.Status = "Failed";
            _unitOfWork.Repository<PaymentTransaction>().Update(transaction);
            await _unitOfWork.SaveChangesAsync();

            await _orderService.CancelPendingOrderByIdAsync(transaction.OrderId);
            return Result<string>.Ok(null, "Payment cancelled.");
        }

        public async Task<Result<PaymentTransactionDto>> GetTransactionByIdAsync(int transactionDbId)
        {
            var tx = await _unitOfWork.Repository<PaymentTransaction>()
                .GetByIdAsync(transactionDbId);

            if (tx == null) return Result<PaymentTransactionDto>.Fail("Not found.");

            return Result<PaymentTransactionDto>.Ok(new PaymentTransactionDto
            {
                Id               = tx.Id,
                OrderId          = tx.OrderId,
                PaymentGatewayId = tx.PaymentGatewayId,
                TransactionId    = tx.TransactionId,
                SessionRef       = tx.SessionRef,
                Amount           = tx.Amount,
                Currency         = tx.Currency,
                Status           = tx.Status,
                CreatedAt        = tx.CreatedAt
            });
        }
    }
}
