using System.Text.Json;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Cache;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;
using MediatR;
using BrandEntity = CarShop.Domain.Entities.Brand;

namespace CarShop.Application.Features.Brand.Commands.UpdateBrand
{
    public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        private const string AllBrandsKey = "brands:all";
        private static string BrandKey(int id) => $"brands:{id}";

        public UpdateBrandCommandHandler(
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            IAuditLogService auditLogService,
            IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = await _unitOfWork.Repository<BrandEntity>().GetByIdAsync(request.Id);
            if (brand == null)
                return Result<string>.Fail("Brand not found.");

            var oldValues = JsonSerializer.Serialize(new { brand.Id, brand.Name });
            brand.Rename(request.Name!);
            _unitOfWork.Repository<BrandEntity>().Update(brand);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var redis = await _unitOfWork.Repository<IntegrationSetting>()
                .FirstOrDefaultAsync(s => s.ServiceName == "Redis", s => new { s.IsEnabled });
            if (redis != null && redis.IsEnabled)
            {
                await _cacheService.RemoveAsync(AllBrandsKey);
                await _cacheService.RemoveAsync(BrandKey(request.Id));
            }

            await _auditLogService.LogAsync("Brand", "Update", _userContextService.UserId, _userContextService.Email,
                $"Updated brand: {brand.Name} (Id: {request.Id})",
                entityId: request.Id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                oldValues: oldValues,
                newValues: JsonSerializer.Serialize(new { brand.Id, brand.Name }));

            return Result<string>.Ok(null, "Brand updated successfully.");
        }
    }
}
