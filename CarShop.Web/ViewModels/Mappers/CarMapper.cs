using CarShop.Application.DTOs.Car;
using CarShop.Web.ViewModels.Car;

namespace CarShop.Web.ViewModels.Mappers
{
    public static class CarMapper
    {
        public static CarViewModel ToViewModel(CarDto dto)
        {
            if (dto == null) return null!;

            return new CarViewModel
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                Quantity = dto.Quantity,
                BrandId = dto.BrandId,
                ImageUrl = dto.ImageUrl
            };
        }

        public static IEnumerable<CarViewModel> ToViewModels(IEnumerable<CarDto> dtos)
        {
            return dtos.Select(ToViewModel);
        }

        public static CarDto ToDto(CarViewModel vm)
        {
            if (vm == null) return null!;

            return new CarDto
            {
                Id = vm.Id,
                Title = vm.Title,
                Description = vm.Description,
                Price = vm.Price,
                Quantity = vm.Quantity,
                BrandId = vm.BrandId,
                ImageUrl = vm.ImageUrl
            };
        }
    }
}
