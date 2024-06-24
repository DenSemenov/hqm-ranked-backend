using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_database.Migrations
{
    /// <inheritdoc />
    public partial class AddedDiscordAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DiscordAppClientId",
                table: "Settings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "DiscordId",
                table: "Players",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DiscordNickname",
                table: "Players",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscordAppClientId",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "DiscordId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "DiscordNickname",
                table: "Players");
        }
    }
}
