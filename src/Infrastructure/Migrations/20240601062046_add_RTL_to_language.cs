using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Offers.CleanArchitecture.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_RTL_to_language : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RTL",
                table: "Languages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RTL",
                table: "Languages");
        }
    }
}
