using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Service.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MovePricesToServiceTariffs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidFrom",
                table: "ServiceTariffs",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: DateTimeOffset.MinValue);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidTo",
                table: "ServiceTariffs",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "ServiceTariffItems",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql(
                """
                INSERT INTO [ServiceTariffs] ([SalonId], [Name], [ValidFrom], [ValidTo], [Deleted], [Status])
                SELECT [SalonId], CONCAT(N'تعرفه منتقل‌شده ', [Id]), [ValidFrom], [ValidTo], [Deleted], [Status]
                FROM [ServicePrices];

                INSERT INTO [ServiceTariffItems] ([SalonId], [TariffId], [ServiceId], [Amount], [Deleted], [Status])
                SELECT price.[SalonId], tariff.[Id], price.[ServiceId], price.[Amount], price.[Deleted], price.[Status]
                FROM [ServicePrices] AS price
                INNER JOIN [ServiceTariffs] AS tariff
                    ON tariff.[SalonId] = price.[SalonId]
                    AND tariff.[Name] = CONCAT(N'تعرفه منتقل‌شده ', price.[Id]);
                """);

            migrationBuilder.DropTable(name: "ServicePrices");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ServiceTariffs_Validity",
                table: "ServiceTariffs",
                sql: "[ValidTo] IS NULL OR [ValidTo] > [ValidFrom]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ServiceTariffItems_Amount",
                table: "ServiceTariffItems",
                sql: "[Amount] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServicePrices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalonId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    ValidFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ValidTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicePrices", x => x.Id);
                    table.CheckConstraint("CK_ServicePrices_Amount", "[Amount] >= 0");
                    table.CheckConstraint("CK_ServicePrices_Validity", "[ValidTo] IS NULL OR [ValidTo] > [ValidFrom]");
                    table.ForeignKey(
                        name: "FK_ServicePrices_Services_SalonId_ServiceId",
                        columns: x => new { x.SalonId, x.ServiceId },
                        principalTable: "Services",
                        principalColumns: new[] { "SalonId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServicePrices_SalonId_ServiceId",
                table: "ServicePrices",
                columns: new[] { "SalonId", "ServiceId" });

            migrationBuilder.CreateIndex(
                name: "IX_ServicePrices_ServiceId_ValidFrom",
                table: "ServicePrices",
                columns: new[] { "ServiceId", "ValidFrom" });

            migrationBuilder.Sql(
                """
                INSERT INTO [ServicePrices] ([SalonId], [ServiceId], [Amount], [Deleted], [Status], [ValidFrom], [ValidTo])
                SELECT item.[SalonId], item.[ServiceId], item.[Amount], item.[Deleted], item.[Status], tariff.[ValidFrom], tariff.[ValidTo]
                FROM [ServiceTariffItems] AS item
                INNER JOIN [ServiceTariffs] AS tariff
                    ON tariff.[SalonId] = item.[SalonId]
                    AND tariff.[Id] = item.[TariffId];
                """);

            migrationBuilder.DropCheckConstraint(
                name: "CK_ServiceTariffs_Validity",
                table: "ServiceTariffs");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ServiceTariffItems_Amount",
                table: "ServiceTariffItems");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "ServiceTariffs");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                table: "ServiceTariffs");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "ServiceTariffItems");

        }
    }
}
