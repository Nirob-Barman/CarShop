using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStripeSessionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripeSessionId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9E22B5A0-3D5C-4F5D-B123-987654321DEF",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "52ee59f5-448e-4d38-8e0a-77cf0b8d0ffa", "AQAAAAIAAYagAAAAEOlrz86WASn7DwqqlwfBQoY9t5GTd2+/LGYKXDogO5l/D/S8LOZg7/K9UEP/FOQipQ==", "8d19212d-40ee-4676-91b9-fbc2a5f625bd" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeSessionId",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9E22B5A0-3D5C-4F5D-B123-987654321DEF",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1aa080b0-fe00-4a71-b8ef-16a4e884f31d", "AQAAAAIAAYagAAAAELAhvAx6ZTDmDNSG8mI1i9gbfzG/KaAEU1tzBnQWXwJ6x+Czzi6QnWF13z8IdVpsew==", "27a857d0-36e6-4e26-9407-4b427178e841" });
        }
    }
}
