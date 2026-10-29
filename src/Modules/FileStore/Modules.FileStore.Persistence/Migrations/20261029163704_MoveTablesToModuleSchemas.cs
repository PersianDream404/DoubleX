using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.FileStore.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveTablesToModuleSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "FileStore");

            migrationBuilder.RenameTable(
                name: "FileStores",
                newName: "FileStores",
                newSchema: "FileStore");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "FileStores",
                schema: "FileStore",
                newName: "FileStores");
        }
    }
}
