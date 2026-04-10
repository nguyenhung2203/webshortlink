using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebShortlink.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminFeedbackToCustomDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminFeedback",
                table: "Domains",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminFeedback",
                table: "Domains");
        }
    }
}
