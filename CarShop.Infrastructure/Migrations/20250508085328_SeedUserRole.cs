using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "BDE7B6A0-7F3D-4F5D-B123-987654321DEF", null, "User", "USER" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9E22B5A0-3D5C-4F5D-B123-987654321DEF",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "548028df-b9e6-4e84-b0a9-78b22ca4114c", "admin@gmail.com", "ADMIN@GMAIL.COM", "ADMIN@GMAIL.COM", "AQAAAAIAAYagAAAAEHWuXvbzTN7UNl034eTilM/xYR+dAlWhQZTBH1wtdsx4I3riYfQXZOqhWAttGZuFww==", "e82df3d9-e9da-48ac-97d2-e7ad3c7f7dad", "admin@gmail.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "BDE7B6A0-7F3D-4F5D-B123-987654321DEF");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9E22B5A0-3D5C-4F5D-B123-987654321DEF",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "20de6def-e2b1-428c-91e7-b259fca221d1", "admin@example.com", "ADMIN@EXAMPLE.COM", "ADMIN@EXAMPLE.COM", "AQAAAAIAAYagAAAAEKu/ja1StyKrNmsV2cntRdoMqNq2Lfv9KDJuoQj1hvb0I0Q4+Q4NfViT/OBzwtFbLg==", "6c8dbe18-68a3-4499-bf40-5c0a77d5b8a0", "admin@example.com" });
        }
    }
}
