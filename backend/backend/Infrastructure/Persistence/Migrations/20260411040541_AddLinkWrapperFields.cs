using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebShortlink.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLinkWrapperFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BrandLogoUrl",
                table: "Links",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BrandName",
                table: "Links",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContinueButtonText",
                table: "Links",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CtaButtonText",
                table: "Links",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CtaButtonUrl",
                table: "Links",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CtaDescription",
                table: "Links",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CtaTitle",
                table: "Links",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DelaySeconds",
                table: "Links",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsWrapperEnabled",
                table: "Links",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RedirectMode",
                table: "Links",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "WarningText",
                table: "Links",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WrapperDescription",
                table: "Links",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WrapperImageUrl",
                table: "Links",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WrapperTheme",
                table: "Links",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WrapperTitle",
                table: "Links",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrandLogoUrl",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "BrandName",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "ContinueButtonText",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "CtaButtonText",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "CtaButtonUrl",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "CtaDescription",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "CtaTitle",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "DelaySeconds",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "IsWrapperEnabled",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "RedirectMode",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "WarningText",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "WrapperDescription",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "WrapperImageUrl",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "WrapperTheme",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "WrapperTitle",
                table: "Links");
        }
    }
}
