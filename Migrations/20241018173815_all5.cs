using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mailo.Migrations
{
    /// <inheritdoc />
    public partial class all5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "AdditionDate", "Quantity" },
                values: new object[] { "18/10/2024 08:38:09 م", 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Product");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "ID",
                keyValue: 1,
                column: "AdditionDate",
                value: "18/10/2024 07:23:18 م");
        }
    }
}
