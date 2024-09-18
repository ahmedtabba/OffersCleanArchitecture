using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataCollector.CleanArchitecture.Infrastructure.Migrations.CleanArchitectureIdentityDb
{
    /// <inheritdoc />
    public partial class fixinterfacenavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ApplicationRoleId",
                table: "ApplicationGroupRoles",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationGroupRoles_ApplicationRoleId",
                table: "ApplicationGroupRoles",
                column: "ApplicationRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationGroupRoles_AspNetRoles_ApplicationRoleId",
                table: "ApplicationGroupRoles",
                column: "ApplicationRoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationGroupRoles_AspNetRoles_ApplicationRoleId",
                table: "ApplicationGroupRoles");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationGroupRoles_ApplicationRoleId",
                table: "ApplicationGroupRoles");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationRoleId",
                table: "ApplicationGroupRoles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
