using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentGatewaySystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StripeSessionId",
                table: "Orders",
                newName: "TransactionRef");

            migrationBuilder.AddColumn<int>(
                name: "PaymentGatewayId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentGateways",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsSandbox = table.Column<bool>(type: "bit", nullable: false),
                    SupportedCurrencies = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Config = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGateways", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    PaymentGatewayId = table.Column<int>(type: "int", nullable: false),
                    SessionRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RawResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_PaymentGateways_PaymentGatewayId",
                        column: x => x.PaymentGatewayId,
                        principalTable: "PaymentGateways",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9E22B5A0-3D5C-4F5D-B123-987654321DEF",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "100e230e-1feb-46b0-9205-ad4bfbd5e2d3", "AQAAAAIAAYagAAAAEGfimMszQg91yekNxGXN45LCRe1wMJhqfzGHAjjVHItc9s9FmWnWSeD9n5NZoQJdtA==", "f1db1606-ee19-42db-8114-fa5ad7cb4860" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentGatewayId",
                table: "Orders",
                column: "PaymentGatewayId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_OrderId",
                table: "PaymentTransactions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_PaymentGatewayId",
                table: "PaymentTransactions",
                column: "PaymentGatewayId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_PaymentGateways_PaymentGatewayId",
                table: "Orders",
                column: "PaymentGatewayId",
                principalTable: "PaymentGateways",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_PaymentGateways_PaymentGatewayId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "PaymentGateways");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PaymentGatewayId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentGatewayId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "TransactionRef",
                table: "Orders",
                newName: "StripeSessionId");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9E22B5A0-3D5C-4F5D-B123-987654321DEF",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "52ee59f5-448e-4d38-8e0a-77cf0b8d0ffa", "AQAAAAIAAYagAAAAEOlrz86WASn7DwqqlwfBQoY9t5GTd2+/LGYKXDogO5l/D/S8LOZg7/K9UEP/FOQipQ==", "8d19212d-40ee-4676-91b9-fbc2a5f625bd" });
        }
    }
}
