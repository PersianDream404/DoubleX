using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Reception.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonnelCommissionOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PersonnelCommissionOutbox",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SalonId = table.Column<long>(type: "bigint", nullable: false),
                    ReceptionId = table.Column<long>(type: "bigint", nullable: false),
                    ReceptionServiceId = table.Column<long>(type: "bigint", nullable: false),
                    OutboxStatus = table.Column<int>(type: "int", nullable: false),
                    Attempts = table.Column<int>(type: "int", nullable: false),
                    NextAttemptAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ProcessedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelCommissionOutbox", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelCommissionOutbox_EventId",
                table: "PersonnelCommissionOutbox",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelCommissionOutbox_OutboxStatus_NextAttemptAt_Id",
                table: "PersonnelCommissionOutbox",
                columns: new[] { "OutboxStatus", "NextAttemptAt", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelCommissionOutbox_ReceptionId_ReceptionServiceId",
                table: "PersonnelCommissionOutbox",
                columns: new[] { "ReceptionId", "ReceptionServiceId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonnelCommissionOutbox");
        }
    }
}
