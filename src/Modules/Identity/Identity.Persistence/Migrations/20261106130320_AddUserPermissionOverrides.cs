using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPermissionOverrides : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPermissionOverrideAudits",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    SalonId = table.Column<long>(type: "bigint", nullable: false),
                    Permission = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Effect = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ActorUserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissionOverrideAudits", x => x.Id);
                    table.CheckConstraint("CK_UserPermissionOverrideAudits_Effect", "[Effect] IS NULL OR [Effect] IN (1, 2)");
                    table.ForeignKey(
                        name: "FK_UserPermissionOverrideAudits_AspNetUsers_ActorUserId",
                        column: x => x.ActorUserId,
                        principalSchema: "Identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPermissionOverrideAudits_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissionOverrides",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    SalonId = table.Column<long>(type: "bigint", nullable: false),
                    Permission = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Effect = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissionOverrides", x => x.Id);
                    table.CheckConstraint("CK_UserPermissionOverrides_Effect", "[Effect] IN (1, 2)");
                    table.ForeignKey(
                        name: "FK_UserPermissionOverrides_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPermissionOverrides_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionOverrideAudits_ActorUserId",
                schema: "Identity",
                table: "UserPermissionOverrideAudits",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionOverrideAudits_UserId_SalonId_CreatedAt",
                schema: "Identity",
                table: "UserPermissionOverrideAudits",
                columns: new[] { "UserId", "SalonId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionOverrides_CreatedBy",
                schema: "Identity",
                table: "UserPermissionOverrides",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionOverrides_UserId_SalonId_Permission",
                schema: "Identity",
                table: "UserPermissionOverrides",
                columns: new[] { "UserId", "SalonId", "Permission" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPermissionOverrideAudits",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserPermissionOverrides",
                schema: "Identity");
        }
    }
}
