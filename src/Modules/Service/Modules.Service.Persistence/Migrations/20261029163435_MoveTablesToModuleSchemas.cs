using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Service.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveTablesToModuleSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Service");

            migrationBuilder.RenameTable(
                name: "ServiceTariffs",
                newName: "ServiceTariffs",
                newSchema: "Service");

            migrationBuilder.RenameTable(
                name: "ServiceTariffItems",
                newName: "ServiceTariffItems",
                newSchema: "Service");

            migrationBuilder.RenameTable(
                name: "Services",
                newName: "Services",
                newSchema: "Service");

            migrationBuilder.RenameTable(
                name: "ServicePersonnels",
                newName: "ServicePersonnels",
                newSchema: "Service");

            migrationBuilder.RenameTable(
                name: "ServiceMaterials",
                newName: "ServiceMaterials",
                newSchema: "Service");

            migrationBuilder.RenameTable(
                name: "ServiceCategories",
                newName: "ServiceCategories",
                newSchema: "Service");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "ServiceTariffs",
                schema: "Service",
                newName: "ServiceTariffs");

            migrationBuilder.RenameTable(
                name: "ServiceTariffItems",
                schema: "Service",
                newName: "ServiceTariffItems");

            migrationBuilder.RenameTable(
                name: "Services",
                schema: "Service",
                newName: "Services");

            migrationBuilder.RenameTable(
                name: "ServicePersonnels",
                schema: "Service",
                newName: "ServicePersonnels");

            migrationBuilder.RenameTable(
                name: "ServiceMaterials",
                schema: "Service",
                newName: "ServiceMaterials");

            migrationBuilder.RenameTable(
                name: "ServiceCategories",
                schema: "Service",
                newName: "ServiceCategories");
        }
    }
}
