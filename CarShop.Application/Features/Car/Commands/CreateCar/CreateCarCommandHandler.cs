using System.Text.Json;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.FileStorage;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Mappers;
using CarShop.Application.Wrappers;
using MediatR;
using CarEntity = CarShop.Domain.Entities.Car;

namespace CarShop.Application.Features.Car.Commands.CreateCar
{
    public class CreateCarCommandHandler : IRequestHandler<CreateCarCommand, Result<int>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorage _fileStorage;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public CreateCarCommandHandler(
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

        public async Task<Result<int>> Handle(CreateCarCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            if (request.File != null)
            {
                dto.ImageUrl = await _fileStorage.UploadFileAsync(request.File.Content!, request.File.FileName!, "uploads/car");
            }

            var car = CarMapper.ToEntity(dto);
            await _unitOfWork.Repository<CarEntity>().AddAsync(car);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync("Car", "Create", _userContextService.UserId, _userContextService.Email,
                $"Created car: {car.Title} (Id: {car.Id})",
                entityId: car.Id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                newValues: JsonSerializer.Serialize(CarMapper.ToDto(car)));

            return Result<int>.Ok(car.Id, "Car created successfully.");
        }
    }
}
