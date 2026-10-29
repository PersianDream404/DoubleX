using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Finance.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveTablesToModuleSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Finance");

            migrationBuilder.RenameTable(
                name: "FinancePersonnelSettlements",
                newName: "FinancePersonnelSettlements",
                newSchema: "Finance");

            migrationBuilder.RenameTable(
                name: "FinancePersonnelSettlementItems",
                newName: "FinancePersonnelSettlementItems",
                newSchema: "Finance");

            migrationBuilder.RenameTable(
                name: "FinancePersonnelEarnings",
                newName: "FinancePersonnelEarnings",
                newSchema: "Finance");

            migrationBuilder.RenameTable(
                name: "FinanceExpenses",
                newName: "FinanceExpenses",
                newSchema: "Finance");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "FinancePersonnelSettlements",
                schema: "Finance",
                newName: "FinancePersonnelSettlements");

            migrationBuilder.RenameTable(
                name: "FinancePersonnelSettlementItems",
                schema: "Finance",
                newName: "FinancePersonnelSettlementItems");

            migrationBuilder.RenameTable(
                name: "FinancePersonnelEarnings",
                schema: "Finance",
                newName: "FinancePersonnelEarnings");

            migrationBuilder.RenameTable(
                name: "FinanceExpenses",
                schema: "Finance",
                newName: "FinanceExpenses");
        }
    }
}
