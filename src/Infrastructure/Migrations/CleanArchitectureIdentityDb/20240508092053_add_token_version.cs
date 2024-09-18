using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataCollector.CleanArchitecture.Infrastructure.Migrations.CleanArchitectureIdentityDb
{
    /// <inheritdoc />
    public partial class add_token_version : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TokenVersion",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenVersion",
                table: "AspNetUsers");
        }
    }
}
