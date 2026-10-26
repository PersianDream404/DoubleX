using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Reception.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReceptionRevision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Revision",
                table: "Receptions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Revision",
                table: "Receptions");
        }
    }
}
