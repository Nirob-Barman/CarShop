using CarShop.Application.Interfaces;
using CarShop.Application.DTOs.Car;
using CarShop.Application.Mappers;
using CarShop.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Features.Car.Queries.SearchCars
{
    public class SearchCarsQueryHandler : IRequestHandler<SearchCarsQuery, Result<PagedResult<CarDto>>>
    {
        private readonly IApplicationDbContext _context;

        public SearchCarsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<CarDto>>> Handle(SearchCarsQuery request, CancellationToken cancellationToken)
        {
            var cars = await _context.Cars.AsNoTracking().Include(c => c.Brand).Where(c =>
                    (string.IsNullOrEmpty(request.Keyword) ||
                        (c.Title != null && c.Title.ToLower().Contains(request.Keyword.ToLower())) ||
                        (c.Description != null && c.Description.ToLower().Contains(request.Keyword.ToLower()))) &&
                    (string.IsNullOrEmpty(request.BrandName) ||
                        (c.Brand != null && c.Brand.Name != null && c.Brand.Name.ToLower() == request.BrandName.ToLower())) &&
                    (!request.MinPrice.HasValue || c.Price >= request.MinPrice.Value) &&
                    (!request.MaxPrice.HasValue || c.Price <= request.MaxPrice.Value))
                .ToListAsync(cancellationToken);

            var carList = cars.ToList();

            // Sorting
            carList = request.SortBy?.ToLower() switch
            {
                "price_asc" => carList.OrderBy(c => c.Price).ToList(),
                "price_desc" => carList.OrderByDescending(c => c.Price).ToList(),
                "title" => carList.OrderBy(c => c.Title).ToList(),
                _ => carList.OrderByDescending(c => c.Id).ToList() // "newest" default
            };

            var totalCount = carList.Count;
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var pagedItems = carList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(CarMapper.ToDto)
                .ToList();

            return Result<PagedResult<CarDto>>.Ok(new PagedResult<CarDto>
            {
                Items = pagedItems,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }
    }
}
