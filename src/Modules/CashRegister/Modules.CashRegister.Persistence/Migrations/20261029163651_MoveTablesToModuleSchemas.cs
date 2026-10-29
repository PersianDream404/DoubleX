using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.CashRegister.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveTablesToModuleSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "CashRegister");

            migrationBuilder.RenameTable(
                name: "CashRegisterTransactions",
                newName: "CashRegisterTransactions",
                newSchema: "CashRegister");

            migrationBuilder.RenameTable(
                name: "CashRegisterSessions",
                newName: "CashRegisterSessions",
                newSchema: "CashRegister");

            migrationBuilder.RenameTable(
                name: "CashRegisters",
                newName: "CashRegisters",
                newSchema: "CashRegister");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "CashRegisterTransactions",
                schema: "CashRegister",
                newName: "CashRegisterTransactions");

            migrationBuilder.RenameTable(
                name: "CashRegisterSessions",
                schema: "CashRegister",
                newName: "CashRegisterSessions");

            migrationBuilder.RenameTable(
                name: "CashRegisters",
                schema: "CashRegister",
                newName: "CashRegisters");
        }
    }
}
