using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Customer.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnforceUniqueCustomerMobile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Customers_SalonId_MobilePhone",
                table: "Customers",
                columns: new[] { "SalonId", "MobilePhone" },
                unique: true,
                filter: "[Deleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_SalonId_MobilePhone",
                table: "Customers");
        }
    }
}
