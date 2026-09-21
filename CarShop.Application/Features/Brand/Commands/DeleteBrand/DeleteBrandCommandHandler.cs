using System.Text.Json;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Cache;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Brand.Commands.DeleteBrand
{
    public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICacheService _cacheService;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        private const string AllBrandsKey = "brands:all";
        private static string BrandKey(int id) => $"brands:{id}";

        public DeleteBrandCommandHandler(
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

        public async Task<Result<string>> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = await _context.Brands.FindAsync(request.Id);
            if (brand == null)
                return Result<string>.Fail("Brand not found.");

            var oldValues = JsonSerializer.Serialize(new { brand.Id, brand.Name });
            _context.Brands.Remove(brand);
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

            await _auditLogService.LogAsync("Brand", "Delete", _userContextService.UserId, _userContextService.Email,
                $"Deleted brand: {brand.Name} (Id: {request.Id})",
                entityId: request.Id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                oldValues: oldValues);

            return Result<string>.Ok(null, "Brand deleted successfully.");
        }
    }
}
