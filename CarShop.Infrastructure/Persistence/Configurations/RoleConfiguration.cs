
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Infrastructure.Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            var adminRoleId = "8D76A8C0-5E5C-4D12-A0C9-123456789ABC";

            builder.HasData(
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = "BDE7B6A0-7F3D-4F5D-B123-987654321DEF",
                    Name = "User",
                    NormalizedName = "USER"
                }
            );
        }
    }
}
