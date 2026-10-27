using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Reception.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCashRegisterPaymentOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CashRegisterId",
                table: "ReceptionPayments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CashRegisterPaymentOutbox",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalonId = table.Column<long>(type: "bigint", nullable: false),
                    CashRegisterId = table.Column<long>(type: "bigint", nullable: false),
                    CashRegisterSessionId = table.Column<long>(type: "bigint", nullable: false),
                    ReceptionId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OccurredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_CashRegisterPaymentOutbox", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceptionPayments_CashRegisterId",
                table: "ReceptionPayments",
                column: "CashRegisterId");

            migrationBuilder.CreateIndex(
                name: "IX_CashRegisterPaymentOutbox_OutboxStatus_NextAttemptAt_Id",
                table: "CashRegisterPaymentOutbox",
                columns: new[] { "OutboxStatus", "NextAttemptAt", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_CashRegisterPaymentOutbox_ReceptionId_IdempotencyKey",
                table: "CashRegisterPaymentOutbox",
                columns: new[] { "ReceptionId", "IdempotencyKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashRegisterPaymentOutbox");

            migrationBuilder.DropIndex(
                name: "IX_ReceptionPayments_CashRegisterId",
                table: "ReceptionPayments");

            migrationBuilder.DropColumn(
                name: "CashRegisterId",
                table: "ReceptionPayments");
        }
    }
}
