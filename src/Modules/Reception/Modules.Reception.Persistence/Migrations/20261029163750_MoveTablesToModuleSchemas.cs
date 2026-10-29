using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Reception.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveTablesToModuleSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Reception");

            migrationBuilder.RenameTable(
                name: "ReceptionServices",
                newName: "ReceptionServices",
                newSchema: "Reception");

            migrationBuilder.RenameTable(
                name: "ReceptionServiceMaterials",
                newName: "ReceptionServiceMaterials",
                newSchema: "Reception");

            migrationBuilder.RenameTable(
                name: "Receptions",
                newName: "Receptions",
                newSchema: "Reception");

            migrationBuilder.RenameTable(
                name: "ReceptionPayments",
                newName: "ReceptionPayments",
                newSchema: "Reception");

            migrationBuilder.RenameTable(
                name: "PersonnelCommissionOutbox",
                newName: "PersonnelCommissionOutbox",
                newSchema: "Reception");

            migrationBuilder.RenameTable(
                name: "CashRegisterPaymentOutbox",
                newName: "CashRegisterPaymentOutbox",
                newSchema: "Reception");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "ReceptionServices",
                schema: "Reception",
                newName: "ReceptionServices");

            migrationBuilder.RenameTable(
                name: "ReceptionServiceMaterials",
                schema: "Reception",
                newName: "ReceptionServiceMaterials");

            migrationBuilder.RenameTable(
                name: "Receptions",
                schema: "Reception",
                newName: "Receptions");

            migrationBuilder.RenameTable(
                name: "ReceptionPayments",
                schema: "Reception",
                newName: "ReceptionPayments");

            migrationBuilder.RenameTable(
                name: "PersonnelCommissionOutbox",
                schema: "Reception",
                newName: "PersonnelCommissionOutbox");

            migrationBuilder.RenameTable(
                name: "CashRegisterPaymentOutbox",
                schema: "Reception",
                newName: "CashRegisterPaymentOutbox");
        }
    }
}
