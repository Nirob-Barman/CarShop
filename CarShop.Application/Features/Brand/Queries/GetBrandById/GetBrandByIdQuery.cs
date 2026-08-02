using CarShop.Application.DTOs.Brand;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Brand.Queries.GetBrandById
{
    public class GetBrandByIdQuery : IRequest<Result<BrandDto>>
    {
        public int Id { get; set; }

        public GetBrandByIdQuery(int id)
        {
            Id = id;
        }
    }
}
