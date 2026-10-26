using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Reception.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TrackMaterialReversal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MovementReversed",
                table: "ReceptionServiceMaterials",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MovementReversed",
                table: "ReceptionServiceMaterials");
        }
    }
}
