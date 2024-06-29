using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_database.Migrations
{
    /// <inheritdoc />
    public partial class AddedCountGames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameInvites_Games_GameId",
                table: "GameInvites");

            migrationBuilder.DropIndex(
                name: "IX_GameInvites_GameId",
                table: "GameInvites");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "GameInvites");

            migrationBuilder.AddColumn<int>(
                name: "GamesCount",
                table: "GameInvites",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GameGameInvites",
                columns: table => new
                {
                    GameInvitesId = table.Column<Guid>(type: "uuid", nullable: false),
                    GamesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameGameInvites", x => new { x.GameInvitesId, x.GamesId });
                    table.ForeignKey(
                        name: "FK_GameGameInvites_GameInvites_GameInvitesId",
                        column: x => x.GameInvitesId,
                        principalTable: "GameInvites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameGameInvites_Games_GamesId",
                        column: x => x.GamesId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameGameInvites_GamesId",
                table: "GameGameInvites",
                column: "GamesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameGameInvites");

            migrationBuilder.DropColumn(
                name: "GamesCount",
                table: "GameInvites");

            migrationBuilder.AddColumn<Guid>(
                name: "GameId",
                table: "GameInvites",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameInvites_GameId",
                table: "GameInvites",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameInvites_Games_GameId",
                table: "GameInvites",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");
        }
    }
}
