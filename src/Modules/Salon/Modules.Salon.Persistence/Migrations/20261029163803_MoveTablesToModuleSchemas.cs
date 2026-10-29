using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Salon.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveTablesToModuleSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Salon");

            migrationBuilder.RenameTable(
                name: "Salons",
                newName: "Salons",
                newSchema: "Salon");

            migrationBuilder.RenameTable(
                name: "Personnels",
                newName: "Personnels",
                newSchema: "Salon");

            migrationBuilder.RenameTable(
                name: "PersonnelPicture",
                newName: "PersonnelPicture",
                newSchema: "Salon");

            migrationBuilder.RenameTable(
                name: "PersonnelPersonalInfos",
                newName: "PersonnelPersonalInfos",
                newSchema: "Salon");

            migrationBuilder.RenameTable(
                name: "PersonnelJobInfos",
                newName: "PersonnelJobInfos",
                newSchema: "Salon");

            migrationBuilder.RenameTable(
                name: "PersonnelHealthTestPicture",
                newName: "PersonnelHealthTestPicture",
                newSchema: "Salon");

            migrationBuilder.RenameTable(
                name: "PersonnelHealthInfo",
                newName: "PersonnelHealthInfo",
                newSchema: "Salon");

            migrationBuilder.RenameTable(
                name: "PersonnelHealthCoursePicture",
                newName: "PersonnelHealthCoursePicture",
                newSchema: "Salon");

            migrationBuilder.RenameTable(
                name: "PersonnelGuarantorPictures",
                newName: "PersonnelGuarantorPictures",
                newSchema: "Salon");

            migrationBuilder.RenameTable(
                name: "PersonnelGuarantorInfo",
                newName: "PersonnelGuarantorInfo",
                newSchema: "Salon");

            migrationBuilder.RenameTable(
                name: "PersonnelFinancialInfos",
                newName: "PersonnelFinancialInfos",
                newSchema: "Salon");

            migrationBuilder.RenameTable(
                name: "PersonnelContracts",
                newName: "PersonnelContracts",
                newSchema: "Salon");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Salons",
                schema: "Salon",
                newName: "Salons");

            migrationBuilder.RenameTable(
                name: "Personnels",
                schema: "Salon",
                newName: "Personnels");

            migrationBuilder.RenameTable(
                name: "PersonnelPicture",
                schema: "Salon",
                newName: "PersonnelPicture");

            migrationBuilder.RenameTable(
                name: "PersonnelPersonalInfos",
                schema: "Salon",
                newName: "PersonnelPersonalInfos");

            migrationBuilder.RenameTable(
                name: "PersonnelJobInfos",
                schema: "Salon",
                newName: "PersonnelJobInfos");

            migrationBuilder.RenameTable(
                name: "PersonnelHealthTestPicture",
                schema: "Salon",
                newName: "PersonnelHealthTestPicture");

            migrationBuilder.RenameTable(
                name: "PersonnelHealthInfo",
                schema: "Salon",
                newName: "PersonnelHealthInfo");

            migrationBuilder.RenameTable(
                name: "PersonnelHealthCoursePicture",
                schema: "Salon",
                newName: "PersonnelHealthCoursePicture");

            migrationBuilder.RenameTable(
                name: "PersonnelGuarantorPictures",
                schema: "Salon",
                newName: "PersonnelGuarantorPictures");

            migrationBuilder.RenameTable(
                name: "PersonnelGuarantorInfo",
                schema: "Salon",
                newName: "PersonnelGuarantorInfo");

            migrationBuilder.RenameTable(
                name: "PersonnelFinancialInfos",
                schema: "Salon",
                newName: "PersonnelFinancialInfos");

            migrationBuilder.RenameTable(
                name: "PersonnelContracts",
                schema: "Salon",
                newName: "PersonnelContracts");
        }
    }
}
