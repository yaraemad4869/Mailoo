using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mailo.Migrations
{
    /// <inheritdoc />
    public partial class all3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "ID", "AdditionDate", "Category", "Description", "Discount", "ImageUrl", "Name", "Price", "dbImage" },
                values: new object[] { 1, "18/10/2024 07:17:27 م", 0, "Designed for comfort and style, the Mailo Pants offer a relaxed fit with soft, breathable fabric—your go-to for any occasion.", 0m, "assets/blackpants1.jpeg", "Mailo basha pants", 750m, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "ID",
                keyValue: 1);
        }
    }
}
