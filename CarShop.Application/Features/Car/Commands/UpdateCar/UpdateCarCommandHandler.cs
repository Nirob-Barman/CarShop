using System.Text.Json;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.FileStorage;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Mappers;
using CarShop.Application.Wrappers;
using MediatR;
using CarEntity = CarShop.Domain.Entities.Car;

namespace CarShop.Application.Features.Car.Commands.UpdateCar
{
    public class UpdateCarCommandHandler : IRequestHandler<UpdateCarCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorage _fileStorage;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public UpdateCarCommandHandler(
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

        public async Task<Result<string>> Handle(UpdateCarCommand request, CancellationToken cancellationToken)
        {
            var car = await _unitOfWork.Repository<CarEntity>().GetByIdAsync(request.Id);
            if (car == null)
                return Result<string>.Fail("Car not found.");

            var oldValues = JsonSerializer.Serialize(CarMapper.ToDto(car));
            var dto = request.Dto;

            if (request.File != null)
            {
                if (!string.IsNullOrEmpty(car.ImageUrl))
                    await _fileStorage.DeleteFileAsync(car.ImageUrl);

                dto.ImageUrl = await _fileStorage.UploadFileAsync(request.File.Content!, request.File.FileName!, "uploads/car");
            }
            else
            {
                // No new image uploaded — keep the existing one
                dto.ImageUrl = car.ImageUrl;
            }

            CarMapper.UpdateEntity(car, dto);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync("Car", "Update", _userContextService.UserId, _userContextService.Email,
                $"Updated car: {car.Title} (Id: {car.Id})",
                entityId: car.Id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                oldValues: oldValues,
                newValues: JsonSerializer.Serialize(CarMapper.ToDto(car)));

            return Result<string>.Ok(null, "Car updated successfully.");
        }
    }
}
