using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Service.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceTariffs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceTariffs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalonId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTariffs", x => x.Id);
                    table.UniqueConstraint("AK_ServiceTariffs_SalonId_Id", x => new { x.SalonId, x.Id });
                    table.ForeignKey(
                        name: "FK_ServiceTariffs_Salons_SalonId",
                        column: x => x.SalonId,
                        principalTable: "Salons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTariffItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalonId = table.Column<long>(type: "bigint", nullable: false),
                    TariffId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTariffItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceTariffItems_ServiceTariffs_SalonId_TariffId",
                        columns: x => new { x.SalonId, x.TariffId },
                        principalTable: "ServiceTariffs",
                        principalColumns: new[] { "SalonId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceTariffItems_Services_SalonId_ServiceId",
                        columns: x => new { x.SalonId, x.ServiceId },
                        principalTable: "Services",
                        principalColumns: new[] { "SalonId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTariffItems_SalonId_ServiceId",
                table: "ServiceTariffItems",
                columns: new[] { "SalonId", "ServiceId" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTariffItems_SalonId_TariffId",
                table: "ServiceTariffItems",
                columns: new[] { "SalonId", "TariffId" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTariffItems_TariffId_ServiceId",
                table: "ServiceTariffItems",
                columns: new[] { "TariffId", "ServiceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTariffs_SalonId_Name",
                table: "ServiceTariffs",
                columns: new[] { "SalonId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceTariffItems");

            migrationBuilder.DropTable(
                name: "ServiceTariffs");
        }
    }
}
