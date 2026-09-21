using System.Text.Json;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.FileStorage;
using CarShop.Application.Mappers;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Car.Commands.DeleteCar
{
    public class DeleteCarCommandHandler : IRequestHandler<DeleteCarCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IFileStorage _fileStorage;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public DeleteCarCommandHandler(
            IApplicationDbContext context,
            IFileStorage fileStorage,
            IAuditLogService auditLogService,
            IUserContextService userContextService)
        {
            _context = context;
            _fileStorage = fileStorage;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(DeleteCarCommand request, CancellationToken cancellationToken)
        {
            var car = await _context.Cars.FindAsync(request.Id);
            if (car == null)
                return Result<string>.Fail("Car not found.");

            if (!string.IsNullOrEmpty(car.ImageUrl))
                await _fileStorage.DeleteFileAsync(car.ImageUrl);

            var oldValues = JsonSerializer.Serialize(CarMapper.ToDto(car));

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync(cancellationToken);

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
