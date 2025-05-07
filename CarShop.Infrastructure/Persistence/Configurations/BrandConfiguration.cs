
using CarShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarShop.Infrastructure.Persistence.Configurations
{
    public class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.HasData(
                new Brand { Id = 1, Name = "Toyota" },
                new Brand { Id = 2, Name = "BMW" },
                new Brand { Id = 3, Name = "Tesla" },
                new Brand { Id = 4, Name = "Ford" }
            );
        }
    }
}
