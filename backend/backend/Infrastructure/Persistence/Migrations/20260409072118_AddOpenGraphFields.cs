using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebShortlink.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOpenGraphFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OgDescription",
                table: "Links",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OgImageUrl",
                table: "Links",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OgTitle",
                table: "Links",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OgDescription",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "OgImageUrl",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "OgTitle",
                table: "Links");
        }
    }
}
