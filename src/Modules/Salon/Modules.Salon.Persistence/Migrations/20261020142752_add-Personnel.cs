using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Salon.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addPersonnel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Salons",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Personnels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalonId = table.Column<long>(type: "bigint", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personnels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Personnels_Salons_SalonId",
                        column: x => x.SalonId,
                        principalTable: "Salons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "PersonnelGuarantorInfo",
                columns: table => new
                {
                    PersonnelId = table.Column<long>(type: "bigint", nullable: false),
                    GuarantorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Relationship = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    GuarantorMobilePhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GuarantorLandlinePhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelGuarantorInfo", x => x.PersonnelId);
                    table.ForeignKey(
                        name: "FK_PersonnelGuarantorInfo_Personnels_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelHealthInfo",
                columns: table => new
                {
                    PersonnelId = table.Column<long>(type: "bigint", nullable: false),
                    HealthStatus = table.Column<bool>(type: "bit", maxLength: 500, nullable: false),
                    AllergyStatus = table.Column<bool>(type: "bit", maxLength: 500, nullable: false),
                    Diseases = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Allergies = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelHealthInfo", x => x.PersonnelId);
                    table.ForeignKey(
                        name: "FK_PersonnelHealthInfo_Personnels_PersonnelId",
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
                    JobPictures = table.Column<string>(type: "nvarchar(max)", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "PersonnelGuarantorPictures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PersonnelId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelGuarantorPictures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelGuarantorPictures_PersonnelGuarantorInfo_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "PersonnelGuarantorInfo",
                        principalColumn: "PersonnelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelHealthCoursePicture",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PersonnelId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelHealthCoursePicture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelHealthCoursePicture_PersonnelHealthInfo_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "PersonnelHealthInfo",
                        principalColumn: "PersonnelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelHealthTestPicture",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PersonnelId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelHealthTestPicture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelHealthTestPicture_PersonnelHealthInfo_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "PersonnelHealthInfo",
                        principalColumn: "PersonnelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelPicture",
                columns: table => new
                {
                    PersonalInfoPersonnelId = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelPicture", x => x.PersonalInfoPersonnelId);
                    table.ForeignKey(
                        name: "FK_PersonnelPicture_PersonnelPersonalInfos_PersonalInfoPersonnelId",
                        column: x => x.PersonalInfoPersonnelId,
                        principalTable: "PersonnelPersonalInfos",
                        principalColumn: "PersonnelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelGuarantorPictures_PersonnelId",
                table: "PersonnelGuarantorPictures",
                column: "PersonnelId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelHealthCoursePicture_PersonnelId",
                table: "PersonnelHealthCoursePicture",
                column: "PersonnelId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelHealthTestPicture_PersonnelId",
                table: "PersonnelHealthTestPicture",
                column: "PersonnelId");

            migrationBuilder.CreateIndex(
                name: "IX_Personnels_SalonId",
                table: "Personnels",
                column: "SalonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonnelFinancialInfos");

            migrationBuilder.DropTable(
                name: "PersonnelGuarantorPictures");

            migrationBuilder.DropTable(
                name: "PersonnelHealthCoursePicture");

            migrationBuilder.DropTable(
                name: "PersonnelHealthTestPicture");

            migrationBuilder.DropTable(
                name: "PersonnelJobInfos");

            migrationBuilder.DropTable(
                name: "PersonnelPicture");

            migrationBuilder.DropTable(
                name: "PersonnelGuarantorInfo");

            migrationBuilder.DropTable(
                name: "PersonnelHealthInfo");

            migrationBuilder.DropTable(
                name: "PersonnelPersonalInfos");

            migrationBuilder.DropTable(
                name: "Personnels");

            migrationBuilder.DropTable(
                name: "Salons");
        }
    }
}
