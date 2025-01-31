using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mailoo.Migrations
{
    /// <inheritdoc />
    public partial class wishlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wishlist_ProductVariant_ProductVariantId",
                table: "Wishlist");

            migrationBuilder.DropIndex(
                name: "IX_Wishlist_ProductVariantId",
                table: "Wishlist");

            migrationBuilder.DropColumn(
                name: "ProductVariantId",
                table: "Wishlist");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "ID",
                keyValue: 2,
                column: "RegistrationDate",
                value: new DateTime(2025, 1, 31, 4, 17, 52, 441, DateTimeKind.Local).AddTicks(1919));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductVariantId",
                table: "Wishlist",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "ID",
                keyValue: 2,
                column: "RegistrationDate",
                value: new DateTime(2025, 1, 31, 4, 13, 4, 251, DateTimeKind.Local).AddTicks(2530));

            migrationBuilder.CreateIndex(
                name: "IX_Wishlist_ProductVariantId",
                table: "Wishlist",
                column: "ProductVariantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlist_ProductVariant_ProductVariantId",
                table: "Wishlist",
                column: "ProductVariantId",
                principalTable: "ProductVariant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
