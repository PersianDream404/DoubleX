using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Reception.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReceptionFinancialStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FinancialStatus",
                table: "Receptions",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql(
                """
                UPDATE reception
                SET FinancialStatus = CASE
                    WHEN paid.Amount <= 0 THEN 1
                    WHEN paid.Amount < reception.TotalAmount THEN 2
                    ELSE 3
                END
                FROM Receptions AS reception
                CROSS APPLY (
                    SELECT COALESCE(SUM(payment.Amount), 0) AS Amount
                    FROM ReceptionPayments AS payment
                    WHERE payment.ReceptionId = reception.Id
                      AND payment.PaymentStatus = 1
                      AND payment.Deleted = 0
                ) AS paid;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinancialStatus",
                table: "Receptions");
        }
    }
}
