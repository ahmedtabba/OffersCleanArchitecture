using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Offers.CleanArchitecture.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_flag_to_country : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FlagPath",
                table: "Countries",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlagPath",
                table: "Countries");
        }
    }
}
