using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Appointment.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveTablesToModuleSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Appointment");

            migrationBuilder.RenameTable(
                name: "Appointments",
                newName: "Appointments",
                newSchema: "Appointment");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Appointments",
                schema: "Appointment",
                newName: "Appointments");
        }
    }
}
