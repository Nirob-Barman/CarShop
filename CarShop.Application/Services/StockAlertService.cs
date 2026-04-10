using CarShop.Application.DTOs.StockAlert;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;

namespace CarShop.Application.Services
{
    public class StockAlertService : IStockAlertService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
        private readonly IUserManager _userManager;
        private readonly IUserContextService _userContextService;

        public StockAlertService(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            INotificationService notificationService,
            IUserManager userManager,
            IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _notificationService = notificationService;
            _userManager = userManager;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> SubscribeAsync(int carId)
        {
            var userId = _userContextService.UserId!;
            var exists = await _unitOfWork.Repository<StockAlert>().AnyAsync(s => s.UserId == userId && s.CarId == carId && !s.IsTriggered);
            if (exists)
                return Result<string>.Fail("You are already subscribed to stock alerts for this car.");

            var car = await _unitOfWork.Repository<Car>().GetByIdAsync(carId);
            if (car == null)
                return Result<string>.Fail("Car not found.");

            var alert = new StockAlert
            {
                UserId = userId,
                CarId = carId,
                IsTriggered = false,
                SubscribedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<StockAlert>().AddAsync(alert);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Ok(null, "You will be notified when this car is back in stock.");
        }

        public async Task<Result<string>> UnsubscribeAsync(int carId)
        {
            var userId = _userContextService.UserId!;
            var alert = await _unitOfWork.Repository<StockAlert>().FirstOrDefaultAsync(
                s => s.UserId == userId && s.CarId == carId && !s.IsTriggered);

            if (alert == null)
                return Result<string>.Fail("No active stock alert found for this car.");

            _unitOfWork.Repository<StockAlert>().Remove(alert);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Ok(null, "Stock alert removed.");
        }

        public async Task<Result<IEnumerable<StockAlertDto>>> GetUserAlertsAsync()
        {
            var userId = _userContextService.UserId!;
            var alerts = await _unitOfWork.Repository<StockAlert>().GetAllWithIncludesAsync(
                predicate: s => s.UserId == userId && !s.IsTriggered,
                selector: s => s,
                s => s.Car!
            );

            var dtos = alerts.Select(s => new StockAlertDto
            {
                Id = s.Id,
                CarId = s.CarId,
                CarTitle = s.Car?.Title,
                CarImageUrl = s.Car?.ImageUrl,
                IsTriggered = s.IsTriggered,
                SubscribedAt = s.SubscribedAt,
                TriggeredAt = s.TriggeredAt
            });

            return Result<IEnumerable<StockAlertDto>>.Ok(dtos);
        }

        public async Task TriggerAlertsForCarAsync(int carId)
        {
            var car = await _unitOfWork.Repository<Car>().GetByIdAsync(carId);
            if (car == null) return;

            var alerts = await _unitOfWork.Repository<StockAlert>().GetAllAsync(
                s => s.CarId == carId && !s.IsTriggered,
                s => s);

            foreach (var alert in alerts)
            {
                alert.IsTriggered = true;
                alert.TriggeredAt = DateTime.UtcNow;
                _unitOfWork.Repository<StockAlert>().Update(alert);

                await _notificationService.CreateNotificationAsync(
                    alert.UserId,
                    $"{car.Title} is back in stock!",
                    $"/Home/Details/{carId}");

                try
                {
                    var user = await _userManager.FindByIdAsync(alert.UserId);
                    if (user?.Email != null)
                    {
                        await _emailService.SendEmailAsync(
                            user.Email,
                            $"{car.Title} is Back in Stock!",
                            $"<h2>Good news!</h2><p>The car <strong>{car.Title}</strong> you were waiting for is now back in stock. <a href='/Home/Details/{carId}'>View it now</a></p>"
                        );
                    }
                }
                catch { /* ignore email failures */ }
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
