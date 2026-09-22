using CarShop.Application.Interfaces;
using CarShop.Application.DTOs.Brand;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Brand.Queries.GetAllBrands
{
    public class GetAllBrandsQueryHandler : IRequestHandler<GetAllBrandsQuery, Result<IEnumerable<BrandDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllBrandsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IEnumerable<BrandDto>>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
        {            
            var brands = await _context.Brands
                .AsNoTracking()
                .Select(b => new BrandDto
                {
                    Id = b.Id,
                    Name = b.Name
                })
                .ToListAsync(cancellationToken);
            return Result<IEnumerable<BrandDto>>.Ok(brands);
        }
    }
}
