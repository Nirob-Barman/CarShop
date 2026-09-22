using CarShop.Application.DTOs.Brand;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Brand.Queries.GetAllBrands
{
    public record GetAllBrandsQuery : IRequest<Result<IEnumerable<BrandDto>>>;
}
