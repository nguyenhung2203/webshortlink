using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebShortlink.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Phase3AnalyticsUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizedSource",
                table: "ClickEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UtmCampaign",
                table: "ClickEvents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UtmMedium",
                table: "ClickEvents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UtmSource",
                table: "ClickEvents",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NormalizedSource",
                table: "ClickEvents");

            migrationBuilder.DropColumn(
                name: "UtmCampaign",
                table: "ClickEvents");

            migrationBuilder.DropColumn(
                name: "UtmMedium",
                table: "ClickEvents");

            migrationBuilder.DropColumn(
                name: "UtmSource",
                table: "ClickEvents");
        }
    }
}
