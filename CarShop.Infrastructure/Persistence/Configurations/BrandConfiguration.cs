
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
                new Brand("Toyota") { Id = 1 },
                new Brand("BMW") { Id = 2 },
                new Brand("Tesla") { Id = 3 },
                new Brand("Ford") { Id = 4 }
            );
        }
    }
}
