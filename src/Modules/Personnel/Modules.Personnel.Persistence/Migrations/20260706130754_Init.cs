using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Personnel.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Personnels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personnels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelDocuments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PersonnelId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelDocuments_Personnels_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelFinancialInfos",
                columns: table => new
                {
                    PersonnelId = table.Column<long>(type: "bigint", nullable: false),
                    SalaryType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelFinancialInfos", x => x.PersonnelId);
                    table.ForeignKey(
                        name: "FK_PersonnelFinancialInfos_Personnels_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelGuarantorInfos",
                columns: table => new
                {
                    PersonnelId = table.Column<long>(type: "bigint", nullable: false),
                    GuarantorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Relationship = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    GuarantorPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelGuarantorInfos", x => x.PersonnelId);
                    table.ForeignKey(
                        name: "FK_PersonnelGuarantorInfos_Personnels_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelHealthInfos",
                columns: table => new
                {
                    PersonnelId = table.Column<long>(type: "bigint", nullable: false),
                    HealthStatus = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AllergyStatus = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelHealthInfos", x => x.PersonnelId);
                    table.ForeignKey(
                        name: "FK_PersonnelHealthInfos_Personnels_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelJobInfos",
                columns: table => new
                {
                    PersonnelId = table.Column<long>(type: "bigint", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PersonnelCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CooperationStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    WorkingShiftType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Skills = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkHistory = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelJobInfos", x => x.PersonnelId);
                    table.ForeignKey(
                        name: "FK_PersonnelJobInfos_Personnels_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelPersonalInfos",
                columns: table => new
                {
                    PersonnelId = table.Column<long>(type: "bigint", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NationalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    MaritalStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChildrenCount = table.Column<int>(type: "int", nullable: false),
                    MobilePhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LandlinePhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AddressText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsCloseToSalon = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelPersonalInfos", x => x.PersonnelId);
                    table.ForeignKey(
                        name: "FK_PersonnelPersonalInfos_Personnels_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelDocuments_PersonnelId",
                table: "PersonnelDocuments",
                column: "PersonnelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonnelDocuments");

            migrationBuilder.DropTable(
                name: "PersonnelFinancialInfos");

            migrationBuilder.DropTable(
                name: "PersonnelGuarantorInfos");

            migrationBuilder.DropTable(
                name: "PersonnelHealthInfos");

            migrationBuilder.DropTable(
                name: "PersonnelJobInfos");

            migrationBuilder.DropTable(
                name: "PersonnelPersonalInfos");

            migrationBuilder.DropTable(
                name: "Personnels");
        }
    }
}
