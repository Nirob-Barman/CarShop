using System.Text.Json;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Cache;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Brand.Commands.UpdateBrand
{
    public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICacheService _cacheService;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        private const string AllBrandsKey = "brands:all";
        private static string BrandKey(int id) => $"brands:{id}";

        public UpdateBrandCommandHandler(
            IApplicationDbContext context,
            ICacheService cacheService,
            IAuditLogService auditLogService,
            IUserContextService userContextService)
        {
            _context = context;
            _cacheService = cacheService;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = await _context.Brands.FindAsync(request.Id);
            if (brand == null)
                return Result<string>.Fail("Brand not found.");

            var oldValues = JsonSerializer.Serialize(new { brand.Id, brand.Name });
            brand.Rename(request.Name!);
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync(cancellationToken);

            var redis = await _context.IntegrationSettings
                .Where(s => s.ServiceName == "Redis")
                .Select(s => new { s.IsEnabled })
                .FirstOrDefaultAsync(cancellationToken);
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
