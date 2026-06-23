using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FurniZone.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Name" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Furniture" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Electronics" },
                    { new Guid("66666666-6666-6666-6666-666666666666"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Home Decor" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "PasswordHash", "Role", "UserName" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@furnizone.com", "$2a$11$HQJECJYJGP.R.fOBlKkqV.ytAHfi0XDYN/2bNqA7N2h8dZEO7b6oO", "Admin", "admin" });

            migrationBuilder.InsertData(
                table: "Carts",
                columns: new[] { "Id", "CreatedAt", "UserId" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.InsertData(
                table: "Wishlists",
                columns: new[] { "Id", "CreatedAt", "UserId" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Carts",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "Wishlists",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));
        }
    }
}
