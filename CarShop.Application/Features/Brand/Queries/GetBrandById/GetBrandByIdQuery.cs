using CarShop.Application.DTOs.Brand;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Brand.Queries.GetBrandById
{
    public record GetBrandByIdQuery(int Id) : IRequest<Result<BrandDto>>;
}
