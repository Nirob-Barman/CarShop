using CarShop.Application.DTOs.Brand;
using CarShop.Web.ViewModels.Brand;

namespace CarShop.Web.ViewModels.Mappers
{
    public static class BrandMapper
    {
        public static BrandViewModel ToViewModel(BrandDto dto)
        {
            if (dto == null) return null!;

            return new BrandViewModel
            {
                Id = dto.Id,
                Name = dto.Name
            };
        }

        public static IEnumerable<BrandViewModel> ToViewModels(IEnumerable<BrandDto> dtos)
        {
            return dtos.Select(ToViewModel);
        }

        public static BrandDto ToDto(this BrandViewModel vm)
        {
            if (vm == null) return null!;

            return new BrandDto
            {
                Id = vm.Id,
                Name = vm.Name?.Trim()
            };
        }
    }
}
