using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Offers.CleanArchitecture.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_OnboardingPages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OnboardingPages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(55)", maxLength: 55, nullable: false),
                    AssetPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnboardingPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OnboardingPageLocalization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanguageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OnboardingPageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OnboardingPageLocalizationFieldType = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnboardingPageLocalization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnboardingPageLocalization_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OnboardingPageLocalization_OnboardingPages_OnboardingPageId",
                        column: x => x.OnboardingPageId,
                        principalTable: "OnboardingPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OnboardingPageLocalization_LanguageId",
                table: "OnboardingPageLocalization",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_OnboardingPageLocalization_OnboardingPageId",
                table: "OnboardingPageLocalization",
                column: "OnboardingPageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OnboardingPageLocalization");

            migrationBuilder.DropTable(
                name: "OnboardingPages");
        }
    }
}
