using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.FileStore.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addremoveIsDeleteFileStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "FileStores");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "FileStores");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "FileStores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "FileStores",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
