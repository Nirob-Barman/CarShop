using CarShop.Application.DTOs.Car;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Car.Queries.SearchCars
{
    public record SearchCarsQuery(
        string? BrandName,
        string? Keyword,
        decimal? MinPrice,
        decimal? MaxPrice,
        string SortBy,
        int Page,
        int PageSize
    ) : IRequest<Result<PagedResult<CarDto>>>;
}
