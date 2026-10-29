using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Inventory.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveTablesToModuleSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Inventory");

            migrationBuilder.RenameTable(
                name: "Units",
                newName: "Units",
                newSchema: "Inventory");

            migrationBuilder.RenameTable(
                name: "StockMovements",
                newName: "StockMovements",
                newSchema: "Inventory");

            migrationBuilder.RenameTable(
                name: "StockBalances",
                newName: "StockBalances",
                newSchema: "Inventory");

            migrationBuilder.RenameTable(
                name: "Materials",
                newName: "Materials",
                newSchema: "Inventory");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Units",
                schema: "Inventory",
                newName: "Units");

            migrationBuilder.RenameTable(
                name: "StockMovements",
                schema: "Inventory",
                newName: "StockMovements");

            migrationBuilder.RenameTable(
                name: "StockBalances",
                schema: "Inventory",
                newName: "StockBalances");

            migrationBuilder.RenameTable(
                name: "Materials",
                schema: "Inventory",
                newName: "Materials");
        }
    }
}
