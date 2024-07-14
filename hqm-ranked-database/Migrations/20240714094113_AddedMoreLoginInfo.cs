using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_database.Migrations
{
    /// <inheritdoc />
    public partial class AddedMoreLoginInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AcceptLang",
                table: "PlayerLogins",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Browser",
                table: "PlayerLogins",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Platform",
                table: "PlayerLogins",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "PlayerLogins",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptLang",
                table: "PlayerLogins");

            migrationBuilder.DropColumn(
                name: "Browser",
                table: "PlayerLogins");

            migrationBuilder.DropColumn(
                name: "Platform",
                table: "PlayerLogins");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "PlayerLogins");
        }
    }
}
