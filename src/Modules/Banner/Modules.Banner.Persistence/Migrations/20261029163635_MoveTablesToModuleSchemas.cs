using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Banner.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveTablesToModuleSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Banner");

            migrationBuilder.RenameTable(
                name: "Banners",
                newName: "Banners",
                newSchema: "Banner");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Banners",
                schema: "Banner",
                newName: "Banners");
        }
    }
}
