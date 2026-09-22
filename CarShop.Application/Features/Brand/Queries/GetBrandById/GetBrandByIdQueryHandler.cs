using CarShop.Application.DTOs.Brand;
using CarShop.Application.Interfaces;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Brand.Queries.GetBrandById
{
    public class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, Result<BrandDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetBrandByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<BrandDto>> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
        {
            var brand = await _context.Brands
                .AsNoTracking()
                .Where(b => b.Id == request.Id)
                .Select(b => new BrandDto
                {
                    Id = b.Id,
                    Name = b.Name
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (brand == null)
                return Result<BrandDto>.Fail("Brand not found");

            return Result<BrandDto>.Ok(brand);
        }
    }
}
