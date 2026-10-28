using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Salon.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MovePersonnelTermsToContracts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PersonnelContracts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonnelId = table.Column<long>(type: "bigint", nullable: false),
                    CooperationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FixedSalaryAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DefaultCommissionRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelContracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelContracts_Personnels_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql("""
                INSERT INTO [PersonnelContracts]
                    ([PersonnelId], [CooperationType], [FixedSalaryAmount], [DefaultCommissionRate],
                     [EffectiveFrom], [EffectiveTo], [IsActive], [Deleted], [Status])
                SELECT
                    financial.[PersonnelId],
                    CASE financial.[SalaryType]
                        WHEN N'Fixed' THEN N'FixedSalary'
                        WHEN N'Commission' THEN N'Commission'
                        WHEN N'FixedAndCommission' THEN N'FixedAndCommission'
                        WHEN N'ChairRental' THEN N'ChairRental'
                        WHEN N'RoomRental' THEN N'RoomRental'
                        ELSE N'Other'
                    END,
                    financial.[FixedSalaryAmount],
                    financial.[CommissionRate],
                    COALESCE(financial.[EffectiveFrom], job.[CooperationStartDate]),
                    financial.[EffectiveTo],
                    CAST(1 AS bit),
                    CAST(0 AS bit),
                    CAST(1 AS bit)
                FROM [PersonnelFinancialInfos] AS financial
                INNER JOIN [PersonnelJobInfos] AS job
                    ON job.[PersonnelId] = financial.[PersonnelId]
                """);

            migrationBuilder.DropColumn(
                name: "CommissionRate",
                table: "PersonnelFinancialInfos");

            migrationBuilder.DropColumn(
                name: "EffectiveFrom",
                table: "PersonnelFinancialInfos");

            migrationBuilder.DropColumn(
                name: "EffectiveTo",
                table: "PersonnelFinancialInfos");

            migrationBuilder.DropColumn(
                name: "FixedSalaryAmount",
                table: "PersonnelFinancialInfos");

            migrationBuilder.DropColumn(
                name: "SalaryType",
                table: "PersonnelFinancialInfos");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelContracts_PersonnelId_EffectiveFrom",
                table: "PersonnelContracts",
                columns: new[] { "PersonnelId", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelContracts_PersonnelId_IsActive",
                table: "PersonnelContracts",
                columns: new[] { "PersonnelId", "IsActive" },
                unique: true,
                filter: "[IsActive] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CommissionRate",
                table: "PersonnelFinancialInfos",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EffectiveFrom",
                table: "PersonnelFinancialInfos",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EffectiveTo",
                table: "PersonnelFinancialInfos",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FixedSalaryAmount",
                table: "PersonnelFinancialInfos",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalaryType",
                table: "PersonnelFinancialInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE financial
                SET financial.[SalaryType] = CASE contract.[CooperationType]
                        WHEN N'FixedSalary' THEN N'Fixed'
                        WHEN N'Commission' THEN N'Commission'
                        WHEN N'FixedAndCommission' THEN N'FixedAndCommission'
                        WHEN N'ChairRental' THEN N'ChairRental'
                        WHEN N'RoomRental' THEN N'RoomRental'
                        ELSE N'ChairRental'
                    END,
                    financial.[FixedSalaryAmount] = contract.[FixedSalaryAmount],
                    financial.[CommissionRate] = contract.[DefaultCommissionRate],
                    financial.[EffectiveFrom] = contract.[EffectiveFrom],
                    financial.[EffectiveTo] = contract.[EffectiveTo]
                FROM [PersonnelFinancialInfos] AS financial
                INNER JOIN [PersonnelContracts] AS contract
                    ON contract.[PersonnelId] = financial.[PersonnelId]
                WHERE contract.[IsActive] = 1
                """);

            migrationBuilder.DropTable(
                name: "PersonnelContracts");
        }
    }
}
