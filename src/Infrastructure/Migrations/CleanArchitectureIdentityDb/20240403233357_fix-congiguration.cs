using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataCollector.CleanArchitecture.Infrastructure.Migrations.CleanArchitectureIdentityDb
{
    /// <inheritdoc />
    public partial class fixcongiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationGroupRole_ApplicationGroups_ApplicationGroupId",
                table: "ApplicationGroupRole");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserGroup_ApplicationGroups_ApplicationGroupId",
                table: "ApplicationUserGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserGroup_AspNetUsers_ApplicationUserId",
                table: "ApplicationUserGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUserGroup",
                table: "ApplicationUserGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationGroupRole",
                table: "ApplicationGroupRole");

            migrationBuilder.RenameTable(
                name: "ApplicationUserGroup",
                newName: "ApplicationUserGroups");

            migrationBuilder.RenameTable(
                name: "ApplicationGroupRole",
                newName: "ApplicationGroupRoles");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserGroup_ApplicationUserId",
                table: "ApplicationUserGroups",
                newName: "IX_ApplicationUserGroups_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserGroup_ApplicationGroupId",
                table: "ApplicationUserGroups",
                newName: "IX_ApplicationUserGroups_ApplicationGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationGroupRole_ApplicationGroupId",
                table: "ApplicationGroupRoles",
                newName: "IX_ApplicationGroupRoles_ApplicationGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUserGroups",
                table: "ApplicationUserGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationGroupRoles",
                table: "ApplicationGroupRoles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationGroupRoles_ApplicationGroups_ApplicationGroupId",
                table: "ApplicationGroupRoles",
                column: "ApplicationGroupId",
                principalTable: "ApplicationGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserGroups_ApplicationGroups_ApplicationGroupId",
                table: "ApplicationUserGroups",
                column: "ApplicationGroupId",
                principalTable: "ApplicationGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserGroups_AspNetUsers_ApplicationUserId",
                table: "ApplicationUserGroups",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationGroupRoles_ApplicationGroups_ApplicationGroupId",
                table: "ApplicationGroupRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserGroups_ApplicationGroups_ApplicationGroupId",
                table: "ApplicationUserGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserGroups_AspNetUsers_ApplicationUserId",
                table: "ApplicationUserGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUserGroups",
                table: "ApplicationUserGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationGroupRoles",
                table: "ApplicationGroupRoles");

            migrationBuilder.RenameTable(
                name: "ApplicationUserGroups",
                newName: "ApplicationUserGroup");

            migrationBuilder.RenameTable(
                name: "ApplicationGroupRoles",
                newName: "ApplicationGroupRole");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserGroups_ApplicationUserId",
                table: "ApplicationUserGroup",
                newName: "IX_ApplicationUserGroup_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserGroups_ApplicationGroupId",
                table: "ApplicationUserGroup",
                newName: "IX_ApplicationUserGroup_ApplicationGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationGroupRoles_ApplicationGroupId",
                table: "ApplicationGroupRole",
                newName: "IX_ApplicationGroupRole_ApplicationGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUserGroup",
                table: "ApplicationUserGroup",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationGroupRole",
                table: "ApplicationGroupRole",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationGroupRole_ApplicationGroups_ApplicationGroupId",
                table: "ApplicationGroupRole",
                column: "ApplicationGroupId",
                principalTable: "ApplicationGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserGroup_ApplicationGroups_ApplicationGroupId",
                table: "ApplicationUserGroup",
                column: "ApplicationGroupId",
                principalTable: "ApplicationGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserGroup_AspNetUsers_ApplicationUserId",
                table: "ApplicationUserGroup",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
