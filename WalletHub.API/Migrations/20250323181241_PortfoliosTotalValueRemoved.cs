using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WalletHub.API.Migrations
{
    /// <inheritdoc />
    public partial class PortfoliosTotalValueRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "492a5597-75fc-46f1-a1b7-7b3872e865f6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fe415542-5b79-4172-8856-3d2dd3375e40");

            migrationBuilder.DropColumn(
                name: "TotalValueUSD",
                table: "Portfolios");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5977b82a-3fa3-48b1-a590-ab73b6d96506", null, "Admin", "ADMIN" },
                    { "77ef11ed-a9d4-4ac1-ad34-5b79222e360e", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5977b82a-3fa3-48b1-a590-ab73b6d96506");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "77ef11ed-a9d4-4ac1-ad34-5b79222e360e");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalValueUSD",
                table: "Portfolios",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "492a5597-75fc-46f1-a1b7-7b3872e865f6", null, "Admin", "ADMIN" },
                    { "fe415542-5b79-4172-8856-3d2dd3375e40", null, "User", "USER" }
                });
        }
    }
}
