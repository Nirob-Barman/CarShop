using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGatewayFamilyToPaymentGateway : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GatewayFamily",
                table: "PaymentGateways",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                UPDATE PaymentGateways
                SET GatewayFamily =
                    CASE
                        WHEN Slug LIKE 'stripe%' THEN 'stripe'
                        WHEN Slug LIKE 'sslcommerz%' THEN 'sslcommerz'
                        WHEN Slug LIKE 'bkash%' THEN 'bkash'
                        WHEN Slug LIKE 'surjopay%' THEN 'surjopay'
                        ELSE LOWER(LEFT(Slug, CHARINDEX('_', Slug + '_') - 1))
                    END
                WHERE ISNULL(GatewayFamily, '') = ''");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9E22B5A0-3D5C-4F5D-B123-987654321DEF",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5d68f71b-7c86-440c-bb1e-66fa0fbbc7aa", "AQAAAAIAAYagAAAAEA5oW8zVMKlKDf+42AW7XUr18ihWfJCe6S07ZOUSuNfRU1MLg1JlD6bT0zi8walmyQ==", "93633c97-1212-4cc6-9b80-811ead545348" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GatewayFamily",
                table: "PaymentGateways");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9E22B5A0-3D5C-4F5D-B123-987654321DEF",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "100e230e-1feb-46b0-9205-ad4bfbd5e2d3", "AQAAAAIAAYagAAAAEGfimMszQg91yekNxGXN45LCRe1wMJhqfzGHAjjVHItc9s9FmWnWSeD9n5NZoQJdtA==", "f1db1606-ee19-42db-8114-fa5ad7cb4860" });
        }
    }
}
