using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataCollector.CleanArchitecture.Infrastructure.Migrations.CleanArchitectureIdentityDb
{
    /// <inheritdoc />
    public partial class add_countryId_toApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "AspNetUsers");
        }
    }
}
