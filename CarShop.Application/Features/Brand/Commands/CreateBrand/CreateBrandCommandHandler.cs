using System.Text.Json;
using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using BrandEntity = CarShop.Domain.Entities.Brand;

namespace CarShop.Application.Features.Brand.Commands.CreateBrand
{
    public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;


        public CreateBrandCommandHandler(
            IApplicationDbContext context,
            IAuditLogService auditLogService,
            IUserContextService userContextService)
        {
            _context = context;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<int>> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = new BrandEntity(request.Name!);
            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync("Brand", "Create", _userContextService.UserId, _userContextService.Email,
                $"Created brand: {brand.Name} (Id: {brand.Id})",
                entityId: brand.Id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                newValues: JsonSerializer.Serialize(new { brand.Id, brand.Name }));

            return Result<int>.Ok(brand.Id, "Brand created successfully.");
        }
    }
}
