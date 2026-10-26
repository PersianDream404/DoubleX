using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Reception.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialReception : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Receptions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalonId = table.Column<long>(type: "bigint", nullable: false),
                    ReceptionNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ReceptionistId = table.Column<long>(type: "bigint", nullable: false),
                    AppointmentId = table.Column<long>(type: "bigint", nullable: true),
                    ReceptionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ReceptionStatus = table.Column<int>(type: "int", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReceptionPayments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceptionId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaidAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceptionPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceptionPayments_Receptions_ReceptionId",
                        column: x => x.ReceptionId,
                        principalTable: "Receptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReceptionServices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceptionId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceTariffId = table.Column<long>(type: "bigint", nullable: false),
                    PersonnelId = table.Column<long>(type: "bigint", nullable: true),
                    ServiceName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FinalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ServiceStatus = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceptionServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceptionServices_Receptions_ReceptionId",
                        column: x => x.ReceptionId,
                        principalTable: "Receptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReceptionServiceMaterials",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceptionServiceId = table.Column<long>(type: "bigint", nullable: false),
                    MaterialId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MovementRegistered = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceptionServiceMaterials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceptionServiceMaterials_ReceptionServices_ReceptionServiceId",
                        column: x => x.ReceptionServiceId,
                        principalTable: "ReceptionServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceptionPayments_ReceptionId_IdempotencyKey",
                table: "ReceptionPayments",
                columns: new[] { "ReceptionId", "IdempotencyKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceptionPayments_ReceptionId_TransactionReference",
                table: "ReceptionPayments",
                columns: new[] { "ReceptionId", "TransactionReference" },
                unique: true,
                filter: "[TransactionReference] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Receptions_SalonId_CustomerId_ReceptionDate",
                table: "Receptions",
                columns: new[] { "SalonId", "CustomerId", "ReceptionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Receptions_SalonId_ReceptionNumber",
                table: "Receptions",
                columns: new[] { "SalonId", "ReceptionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceptionServiceMaterials_MaterialId",
                table: "ReceptionServiceMaterials",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceptionServiceMaterials_ReceptionServiceId_IdempotencyKey",
                table: "ReceptionServiceMaterials",
                columns: new[] { "ReceptionServiceId", "IdempotencyKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceptionServices_ReceptionId_ServiceId",
                table: "ReceptionServices",
                columns: new[] { "ReceptionId", "ServiceId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceptionPayments");

            migrationBuilder.DropTable(
                name: "ReceptionServiceMaterials");

            migrationBuilder.DropTable(
                name: "ReceptionServices");

            migrationBuilder.DropTable(
                name: "Receptions");
        }
    }
}
