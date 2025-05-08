using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminAndRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "8D76A8C0-5E5C-4D12-A0C9-123456789ABC", null, "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "ConcurrencyStamp", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "9E22B5A0-3D5C-4F5D-B123-987654321DEF", 0, null, "20de6def-e2b1-428c-91e7-b259fca221d1", "admin@example.com", true, null, false, null, "ADMIN@EXAMPLE.COM", "ADMIN@EXAMPLE.COM", "AQAAAAIAAYagAAAAEKu/ja1StyKrNmsV2cntRdoMqNq2Lfv9KDJuoQj1hvb0I0Q4+Q4NfViT/OBzwtFbLg==", null, false, "6c8dbe18-68a3-4499-bf40-5c0a77d5b8a0", false, "admin@example.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "8D76A8C0-5E5C-4D12-A0C9-123456789ABC", "9E22B5A0-3D5C-4F5D-B123-987654321DEF" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "8D76A8C0-5E5C-4D12-A0C9-123456789ABC", "9E22B5A0-3D5C-4F5D-B123-987654321DEF" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8D76A8C0-5E5C-4D12-A0C9-123456789ABC");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9E22B5A0-3D5C-4F5D-B123-987654321DEF");
        }
    }
}
