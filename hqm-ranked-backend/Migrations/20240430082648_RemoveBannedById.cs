using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBannedById : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bans_Players_BannedByPlayerId",
                table: "Bans");

            migrationBuilder.DropIndex(
                name: "IX_Bans_BannedByPlayerId",
                table: "Bans");

            migrationBuilder.DropColumn(
                name: "BannedByPlayerId",
                table: "Bans");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BannedByPlayerId",
                table: "Bans",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bans_BannedByPlayerId",
                table: "Bans",
                column: "BannedByPlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bans_Players_BannedByPlayerId",
                table: "Bans",
                column: "BannedByPlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
