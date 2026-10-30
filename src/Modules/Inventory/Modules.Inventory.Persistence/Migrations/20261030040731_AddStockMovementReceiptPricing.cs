using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Inventory.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStockMovementReceiptPricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PurchaseQuantity",
                schema: "Inventory",
                table: "StockMovements",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PurchaseUnitPrice",
                schema: "Inventory",
                table: "StockMovements",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchaseQuantity",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "PurchaseUnitPrice",
                schema: "Inventory",
                table: "StockMovements");
        }
    }
}
