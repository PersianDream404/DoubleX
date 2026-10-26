using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Service.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalonId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCategories", x => x.Id);
                    table.UniqueConstraint("AK_ServiceCategories_SalonId_Id", x => new { x.SalonId, x.Id });
                    table.ForeignKey(
                        name: "FK_ServiceCategories_Salons_SalonId",
                        column: x => x.SalonId,
                        principalTable: "Salons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalonId = table.Column<long>(type: "bigint", nullable: false),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                    table.UniqueConstraint("AK_Services_SalonId_Id", x => new { x.SalonId, x.Id });
                    table.CheckConstraint("CK_Services_DurationMinutes", "[DurationMinutes] > 0");
                    table.ForeignKey(
                        name: "FK_Services_ServiceCategories_SalonId_CategoryId",
                        columns: x => new { x.SalonId, x.CategoryId },
                        principalTable: "ServiceCategories",
                        principalColumns: new[] { "SalonId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceMaterials",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalonId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    MaterialId = table.Column<long>(type: "bigint", nullable: false),
                    QuantityPerService = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceMaterials", x => x.Id);
                    table.CheckConstraint("CK_ServiceMaterials_QuantityPerService", "[QuantityPerService] > 0");
                    table.ForeignKey(
                        name: "FK_ServiceMaterials_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceMaterials_Services_SalonId_ServiceId",
                        columns: x => new { x.SalonId, x.ServiceId },
                        principalTable: "Services",
                        principalColumns: new[] { "SalonId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServicePrices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalonId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ValidFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ValidTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
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
                name: "IX_ServiceCategories_SalonId_Name",
                table: "ServiceCategories",
                columns: new[] { "SalonId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceMaterials_MaterialId",
                table: "ServiceMaterials",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceMaterials_SalonId",
                table: "ServiceMaterials",
                column: "SalonId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceMaterials_SalonId_ServiceId",
                table: "ServiceMaterials",
                columns: new[] { "SalonId", "ServiceId" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceMaterials_ServiceId_MaterialId",
                table: "ServiceMaterials",
                columns: new[] { "ServiceId", "MaterialId" },
                unique: true,
                filter: "[Deleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePrices_SalonId_ServiceId",
                table: "ServicePrices",
                columns: new[] { "SalonId", "ServiceId" });

            migrationBuilder.CreateIndex(
                name: "IX_ServicePrices_ServiceId_ValidFrom",
                table: "ServicePrices",
                columns: new[] { "ServiceId", "ValidFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_Services_SalonId_CategoryId",
                table: "Services",
                columns: new[] { "SalonId", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_Services_SalonId_Code",
                table: "Services",
                columns: new[] { "SalonId", "Code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceMaterials");

            migrationBuilder.DropTable(
                name: "ServicePrices");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "ServiceCategories");
        }
    }
}
