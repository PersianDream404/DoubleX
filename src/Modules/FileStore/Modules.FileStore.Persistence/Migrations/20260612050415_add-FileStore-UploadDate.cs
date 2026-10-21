using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.FileStore.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addFileStoreUploadDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UploadDate",
                table: "FileStores",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadDate",
                table: "FileStores");
        }
    }
}
