using System.Text.Json;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.FileStorage;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Mappers;
using CarShop.Application.Wrappers;
using MediatR;
using CarEntity = CarShop.Domain.Entities.Car;

namespace CarShop.Application.Features.Car.Commands.DeleteCar
{
    public class DeleteCarCommandHandler : IRequestHandler<DeleteCarCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorage _fileStorage;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public DeleteCarCommandHandler(
            IUnitOfWork unitOfWork,
            IFileStorage fileStorage,
            IAuditLogService auditLogService,
            IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(DeleteCarCommand request, CancellationToken cancellationToken)
        {
            var car = await _unitOfWork.Repository<CarEntity>().GetByIdAsync(request.Id);
            if (car == null)
                return Result<string>.Fail("Car not found.");

            if (!string.IsNullOrEmpty(car.ImageUrl))
                await _fileStorage.DeleteFileAsync(car.ImageUrl);

            var oldValues = JsonSerializer.Serialize(CarMapper.ToDto(car));

            _unitOfWork.Repository<CarEntity>().Remove(car);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync("Car", "Delete", _userContextService.UserId, _userContextService.Email,
                $"Deleted car: {car.Title} (Id: {request.Id})",
                entityId: request.Id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                oldValues: oldValues);

            return Result<string>.Ok(null, "Car deleted successfully.");
        }
    }
}
