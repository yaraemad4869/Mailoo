using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mailoo.Migrations
{
    /// <inheritdoc />
    public partial class changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wishlist_ProductVariant_ProductVariantID",
                table: "Wishlist");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Payment");

            migrationBuilder.RenameColumn(
                name: "ProductVariantID",
                table: "Wishlist",
                newName: "ProductVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_Wishlist_ProductVariantID",
                table: "Wishlist",
                newName: "IX_Wishlist_ProductVariantId");

            migrationBuilder.AlterColumn<int>(
                name: "ProductVariantId",
                table: "Wishlist",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "ID",
                keyValue: 2,
                column: "RegistrationDate",
                value: new DateTime(2025, 1, 31, 4, 13, 4, 251, DateTimeKind.Local).AddTicks(2530));

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlist_ProductVariant_ProductVariantId",
                table: "Wishlist",
                column: "ProductVariantId",
                principalTable: "ProductVariant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wishlist_ProductVariant_ProductVariantId",
                table: "Wishlist");

            migrationBuilder.RenameColumn(
                name: "ProductVariantId",
                table: "Wishlist",
                newName: "ProductVariantID");

            migrationBuilder.RenameIndex(
                name: "IX_Wishlist_ProductVariantId",
                table: "Wishlist",
                newName: "IX_Wishlist_ProductVariantID");

            migrationBuilder.AlterColumn<int>(
                name: "ProductVariantID",
                table: "Wishlist",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "Payment",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "Payment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "ID",
                keyValue: 2,
                column: "RegistrationDate",
                value: new DateTime(2025, 1, 31, 1, 28, 59, 699, DateTimeKind.Local).AddTicks(4832));

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlist_ProductVariant_ProductVariantID",
                table: "Wishlist",
                column: "ProductVariantID",
                principalTable: "ProductVariant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
