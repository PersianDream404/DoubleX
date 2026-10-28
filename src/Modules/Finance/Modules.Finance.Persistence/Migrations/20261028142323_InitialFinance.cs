using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Finance.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialFinance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinanceExpenses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalonId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OccurredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CancelledAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceExpenses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FinancePersonnelEarnings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalonId = table.Column<long>(type: "bigint", nullable: false),
                    PersonnelId = table.Column<long>(type: "bigint", nullable: false),
                    ReceptionId = table.Column<long>(type: "bigint", nullable: true),
                    ReceptionServiceId = table.Column<long>(type: "bigint", nullable: true),
                    EarningType = table.Column<int>(type: "int", nullable: false),
                    CalculationBaseAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CommissionRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PeriodStart = table.Column<DateOnly>(type: "date", nullable: true),
                    PeriodEnd = table.Column<DateOnly>(type: "date", nullable: true),
                    OccurredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CalculatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancePersonnelEarnings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FinancePersonnelSettlements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalonId = table.Column<long>(type: "bigint", nullable: false),
                    PersonnelId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    EarningType = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancePersonnelSettlements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FinancePersonnelSettlementItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SettlementId = table.Column<long>(type: "bigint", nullable: false),
                    PersonnelEarningId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancePersonnelSettlementItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancePersonnelSettlementItems_FinancePersonnelEarnings_PersonnelEarningId",
                        column: x => x.PersonnelEarningId,
                        principalTable: "FinancePersonnelEarnings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinancePersonnelSettlementItems_FinancePersonnelSettlements_SettlementId",
                        column: x => x.SettlementId,
                        principalTable: "FinancePersonnelSettlements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinanceExpenses_SalonId_IdempotencyKey",
                table: "FinanceExpenses",
                columns: new[] { "SalonId", "IdempotencyKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinanceExpenses_SalonId_OccurredAt_Status",
                table: "FinanceExpenses",
                columns: new[] { "SalonId", "OccurredAt", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_FinancePersonnelEarnings_SalonId_IdempotencyKey",
                table: "FinancePersonnelEarnings",
                columns: new[] { "SalonId", "IdempotencyKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancePersonnelEarnings_SalonId_PersonnelId_EarningType_OccurredAt",
                table: "FinancePersonnelEarnings",
                columns: new[] { "SalonId", "PersonnelId", "EarningType", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_FinancePersonnelEarnings_SalonId_ReceptionServiceId",
                table: "FinancePersonnelEarnings",
                columns: new[] { "SalonId", "ReceptionServiceId" },
                unique: true,
                filter: "[ReceptionServiceId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FinancePersonnelSettlementItems_PersonnelEarningId",
                table: "FinancePersonnelSettlementItems",
                column: "PersonnelEarningId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancePersonnelSettlementItems_SettlementId_PersonnelEarningId",
                table: "FinancePersonnelSettlementItems",
                columns: new[] { "SettlementId", "PersonnelEarningId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancePersonnelSettlements_SalonId_IdempotencyKey",
                table: "FinancePersonnelSettlements",
                columns: new[] { "SalonId", "IdempotencyKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancePersonnelSettlements_SalonId_PersonnelId_EarningType_PaidAt",
                table: "FinancePersonnelSettlements",
                columns: new[] { "SalonId", "PersonnelId", "EarningType", "PaidAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinanceExpenses");

            migrationBuilder.DropTable(
                name: "FinancePersonnelSettlementItems");

            migrationBuilder.DropTable(
                name: "FinancePersonnelEarnings");

            migrationBuilder.DropTable(
                name: "FinancePersonnelSettlements");
        }
    }
}
