using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Salon.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonnelEmploymentTerms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
