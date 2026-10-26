using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Service.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddServicePersonnels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServicePersonnels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalonId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    PersonnelId = table.Column<long>(type: "bigint", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicePersonnels", x => x.Id);
                    table.UniqueConstraint("AK_ServicePersonnels_SalonId_Id", x => new { x.SalonId, x.Id });
                    table.ForeignKey(
                        name: "FK_ServicePersonnels_Personnels_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServicePersonnels_Services_SalonId_ServiceId",
                        columns: x => new { x.SalonId, x.ServiceId },
                        principalTable: "Services",
                        principalColumns: new[] { "SalonId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServicePersonnels_PersonnelId",
                table: "ServicePersonnels",
                column: "PersonnelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePersonnels_SalonId_ServiceId_PersonnelId",
                table: "ServicePersonnels",
                columns: new[] { "SalonId", "ServiceId", "PersonnelId" },
                unique: true,
                filter: "[Deleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServicePersonnels");
        }
    }
}
