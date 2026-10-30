using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Inventory.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStockMovementFifoCost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                schema: "Inventory",
                table: "StockMovements",
                type: "decimal(38,2)",
                precision: 38,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                schema: "Inventory",
                table: "StockMovements",
                type: "decimal(38,8)",
                precision: 38,
                scale: 8,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCost",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "UnitCost",
                schema: "Inventory",
                table: "StockMovements");
        }
    }
}
