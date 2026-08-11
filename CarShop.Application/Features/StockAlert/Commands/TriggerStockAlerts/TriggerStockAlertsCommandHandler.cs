using CarShop.Application.Features.Notification.Commands.CreateNotification;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using CarEntity = CarShop.Domain.Entities.Car;
using StockAlertEntity = CarShop.Domain.Entities.StockAlert;

namespace CarShop.Application.Features.StockAlert.Commands.TriggerStockAlerts
{
    public class TriggerStockAlertsCommandHandler : IRequestHandler<TriggerStockAlertsCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IMediator _mediator;
        private readonly IUserManager _userManager;

        public TriggerStockAlertsCommandHandler(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IMediator mediator,
            IUserManager userManager)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _mediator = mediator;
            _userManager = userManager;
        }

        public async Task<Result<string>> Handle(TriggerStockAlertsCommand request, CancellationToken cancellationToken)
        {
            var car = await _unitOfWork.Repository<CarEntity>().GetByIdAsync(request.CarId);
            if (car == null)
                return Result<string>.Ok(null, "Car not found.");

            var alerts = await _unitOfWork.Repository<StockAlertEntity>().GetAllAsync(
                s => s.CarId == request.CarId && !s.IsTriggered,
                s => s);

            foreach (var alert in alerts)
            {
                alert.Trigger();
                _unitOfWork.Repository<StockAlertEntity>().Update(alert);

                await _mediator.Send(new CreateNotificationCommand(
                    alert.UserId,
                    $"{car.Title} is back in stock!",
                    $"/Home/Details/{request.CarId}"));

                try
                {
                    var user = await _userManager.FindByIdAsync(alert.UserId);
                    if (user?.Email != null)
                    {
                        await _emailService.SendEmailAsync(
                            user.Email,
                            $"{car.Title} is Back in Stock!",
                            $"<h2>Good news!</h2><p>The car <strong>{car.Title}</strong> you were waiting for is now back in stock. <a href='/Home/Details/{request.CarId}'>View it now</a></p>"
                        );
                    }
                }
                catch { /* ignore email failures */ }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok(null, "Stock alerts triggered.");
        }
    }
}
