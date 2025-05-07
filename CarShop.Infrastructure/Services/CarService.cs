using CarShop.Application.DTOs.Car;
using CarShop.Application.Interfaces;
using CarShop.Application.Mappers;
using CarShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Infrastructure.Services
{
    public class CarService : ICarService
    {
        private readonly AppDbContext _context;

        public CarService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CarDto>> GetAllCarsAsync()
        {
            //return await _context.Cars
            //    .Include(c => c.Brand)
            //    .Select(c => new CarDto
            //    {
            //        Id = c.Id,
            //        Title = c.Title,
            //        Description = c.Description,
            //        Price = c.Price,
            //        Quantity = c.Quantity,
            //        ImageUrl = c.ImageUrl,
            //        BrandName = c.Brand!.Name
            //    })
            //    .ToListAsync();
            var cars = await _context.Cars.Include(c => c.Brand).ToListAsync();
            return cars.Select(CarMapper.ToDto);
        }

        public async Task<IEnumerable<CarDto>> GetCarsByBrandIdAsync(int brandId)
        {
            //return await _context.Cars
            //    .Where(c => c.BrandId == brandId)
            //    .Include(c => c.Brand)
            //    .Select(c => new CarDto
            //    {
            //        Id = c.Id,
            //        Title = c.Title,
            //        Description = c.Description,
            //        Price = c.Price,
            //        Quantity = c.Quantity,
            //        ImageUrl = c.ImageUrl,
            //        BrandName = c.Brand!.Name
            //    })
            //    .ToListAsync();
            var cars = await _context.Cars
                .Where(c => c.BrandId == brandId)
                .Include(c => c.Brand)
                .ToListAsync();
            return cars.Select(CarMapper.ToDto);
        }

        public async Task<CarDto> GetCarByIdAsync(int id)
        {
            var car = await _context.Cars.Include(c => c.Brand).FirstOrDefaultAsync(c => c.Id == id);
            //return car != null ? new CarDto
            //{
            //    Id = car.Id,
            //    Title = car.Title,
            //    Description = car.Description,
            //    Price = car.Price,
            //    Quantity = car.Quantity,
            //    ImageUrl = car.ImageUrl,
            //    BrandName = car.Brand!.Name
            //} : null!;
            return car != null ? CarMapper.ToDto(car) : null!;
        }

        public async Task<int> CreateCarAsync(CarDto dto)
        {
            //var car = new Car
            //{
            //    Title = dto.Title,
            //    Description = dto.Description,
            //    Price = dto.Price,
            //    Quantity = dto.Quantity,
            //    ImageUrl = dto.ImageUrl,
            //    BrandId = dto.BrandId
            //};
            var car = CarMapper.ToEntity(dto);
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return car.Id;
        }

        public async Task UpdateCarAsync(int id, CarDto dto)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                //car.Title = dto.Title;
                //car.Description = dto.Description;
                //car.Price = dto.Price;
                //car.Quantity = dto.Quantity;
                //car.ImageUrl = dto.ImageUrl;
                //car.BrandId = dto.BrandId;
                CarMapper.UpdateEntity(car, dto);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteCarAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }
        }
    }
}
