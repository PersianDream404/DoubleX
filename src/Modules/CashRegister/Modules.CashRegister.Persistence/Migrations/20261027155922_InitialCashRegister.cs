using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.CashRegister.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCashRegister : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CashRegisters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalonId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashRegisters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CashRegisterSessions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalonId = table.Column<long>(type: "bigint", nullable: false),
                    CashRegisterId = table.Column<long>(type: "bigint", nullable: false),
                    OpenedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    OpenedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    OpeningBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ClosedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    ClosedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ActualClosingBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ExpectedClosingBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Difference = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    SessionStatus = table.Column<int>(type: "int", nullable: false),
                    Revision = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashRegisterSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashRegisterSessions_CashRegisters_CashRegisterId",
                        column: x => x.CashRegisterId,
                        principalTable: "CashRegisters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CashRegisterTransactions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalonId = table.Column<long>(type: "bigint", nullable: false),
                    CashRegisterId = table.Column<long>(type: "bigint", nullable: false),
                    SessionId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OccurredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReferenceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReferenceId = table.Column<long>(type: "bigint", nullable: true),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashRegisterTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashRegisterTransactions_CashRegisterSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "CashRegisterSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashRegisterTransactions_CashRegisters_CashRegisterId",
                        column: x => x.CashRegisterId,
                        principalTable: "CashRegisters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashRegisters_SalonId_Code",
                table: "CashRegisters",
                columns: new[] { "SalonId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashRegisters_SalonId_Status",
                table: "CashRegisters",
                columns: new[] { "SalonId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CashRegisterSessions_CashRegisterId",
                table: "CashRegisterSessions",
                column: "CashRegisterId");

            migrationBuilder.CreateIndex(
                name: "IX_CashRegisterSessions_SalonId_CashRegisterId_SessionStatus",
                table: "CashRegisterSessions",
                columns: new[] { "SalonId", "CashRegisterId", "SessionStatus" },
                unique: true,
                filter: "[SessionStatus] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_CashRegisterTransactions_CashRegisterId_OccurredAt",
                table: "CashRegisterTransactions",
                columns: new[] { "CashRegisterId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CashRegisterTransactions_SalonId_IdempotencyKey",
                table: "CashRegisterTransactions",
                columns: new[] { "SalonId", "IdempotencyKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashRegisterTransactions_SessionId_OccurredAt",
                table: "CashRegisterTransactions",
                columns: new[] { "SessionId", "OccurredAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashRegisterTransactions");

            migrationBuilder.DropTable(
                name: "CashRegisterSessions");

            migrationBuilder.DropTable(
                name: "CashRegisters");
        }
    }
}
