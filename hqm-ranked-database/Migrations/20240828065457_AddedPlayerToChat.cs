using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_database.Migrations
{
    /// <inheritdoc />
    public partial class AddedPlayerToChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ReplayChats",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PlayerId",
                table: "ReplayChats",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReplayChats_PlayerId",
                table: "ReplayChats",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReplayChats_Players_PlayerId",
                table: "ReplayChats",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReplayChats_Players_PlayerId",
                table: "ReplayChats");

            migrationBuilder.DropIndex(
                name: "IX_ReplayChats_PlayerId",
                table: "ReplayChats");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ReplayChats");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "ReplayChats");
        }
    }
}
