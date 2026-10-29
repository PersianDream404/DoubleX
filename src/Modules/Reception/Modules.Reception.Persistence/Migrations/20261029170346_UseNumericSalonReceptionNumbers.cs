using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Reception.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UseNumericSalonReceptionNumbers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Receptions_SalonId_ReceptionNumber",
                schema: "Reception",
                table: "Receptions");

            migrationBuilder.Sql(
                """
                ;WITH NumberedReceptions AS
                (
                    SELECT [Id],
                           ROW_NUMBER() OVER (PARTITION BY [SalonId] ORDER BY [Id]) AS [ReceptionNumber]
                    FROM [Reception].[Receptions]
                )
                UPDATE reception
                SET [ReceptionNumber] = CONVERT(nvarchar(40), numbered.[ReceptionNumber])
                FROM [Reception].[Receptions] AS reception
                INNER JOIN NumberedReceptions AS numbered ON numbered.[Id] = reception.[Id];
                """);

            migrationBuilder.AlterColumn<long>(
                name: "ReceptionNumber",
                schema: "Reception",
                table: "Receptions",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.CreateTable(
                name: "ReceptionNumberSequences",
                schema: "Reception",
                columns: table => new
                {
                    SalonId = table.Column<long>(type: "bigint", nullable: false),
                    LastNumber = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceptionNumberSequences", x => x.SalonId);
                });

            migrationBuilder.Sql(
                """
                INSERT INTO [Reception].[ReceptionNumberSequences] ([SalonId], [LastNumber])
                SELECT [SalonId], MAX([ReceptionNumber])
                FROM [Reception].[Receptions]
                GROUP BY [SalonId];
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Receptions_SalonId_ReceptionNumber",
                schema: "Reception",
                table: "Receptions",
                columns: new[] { "SalonId", "ReceptionNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Receptions_SalonId_ReceptionNumber",
                schema: "Reception",
                table: "Receptions");

            migrationBuilder.DropTable(
                name: "ReceptionNumberSequences",
                schema: "Reception");

            migrationBuilder.AlterColumn<string>(
                name: "ReceptionNumber",
                schema: "Reception",
                table: "Receptions",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_Receptions_SalonId_ReceptionNumber",
                schema: "Reception",
                table: "Receptions",
                columns: new[] { "SalonId", "ReceptionNumber" },
                unique: true);
        }
    }
}
