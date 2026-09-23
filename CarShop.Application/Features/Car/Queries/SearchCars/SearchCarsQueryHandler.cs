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
            // Pagination
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize < 1 ? 10 : Math.Min(request.PageSize, 100);

            var query = _context.Cars.AsNoTracking();

            // Filtering
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var keyword = request.Keyword.Trim();

                query = query.Where(c =>
                    (c.Title != null && c.Title.Contains(keyword)) ||
                    (c.Description != null && c.Description.Contains(keyword)));
            }

            if (!string.IsNullOrWhiteSpace(request.BrandName))
            {
                var brandName = request.BrandName.Trim();

                query = query.Where(c =>
                    c.Brand != null &&
                    c.Brand.Name != null &&
                    c.Brand.Name == brandName);
            }

            if (request.MinPrice.HasValue)
            {
                query = query.Where(c => c.Price >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                query = query.Where(c => c.Price <= request.MaxPrice.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            // Sorting
            var sortBy = request.SortBy?.Trim().ToLowerInvariant();
            query = sortBy switch
            {
                "price_asc" =>
                    query.OrderBy(c => c.Price).ThenByDescending(c => c.Id),

                "price_desc" =>
                    query.OrderByDescending(c => c.Price).ThenByDescending(c => c.Id),

                "title" =>
                    query.OrderBy(c => c.Title).ThenByDescending(c => c.Id),

                _ =>
                    query.OrderByDescending(c => c.Id)
            };

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(CarMapper.ToDtoExpression).ToListAsync(cancellationToken);

            return Result<PagedResult<CarDto>>.Ok(new PagedResult<CarDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }
    }
}
