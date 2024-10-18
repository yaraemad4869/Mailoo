using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mailo.Migrations
{
    /// <inheritdoc />
    public partial class all4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "ID",
                keyValue: 1,
                column: "AdditionDate",
                value: "18/10/2024 07:23:18 م");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "ID",
                keyValue: 1,
                column: "AdditionDate",
                value: "18/10/2024 07:17:27 م");
        }
    }
}
