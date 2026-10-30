using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Reception.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReceptionMaterialFifoCost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                schema: "Reception",
                table: "ReceptionServiceMaterials",
                type: "decimal(38,8)",
                precision: 38,
                scale: 8,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                schema: "Reception",
                table: "ReceptionServiceMaterials",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCost",
                schema: "Reception",
                table: "ReceptionServiceMaterials");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                schema: "Reception",
                table: "ReceptionServiceMaterials",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,8)",
                oldPrecision: 38,
                oldScale: 8,
                oldNullable: true);
        }
    }
}
