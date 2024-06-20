using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedRightNotificationsScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PushTokens",
                table: "Players");

            migrationBuilder.AddColumn<int>(
                name: "PlayerId",
                table: "ReplayGoals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "ReplayGoals",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PlayerNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    LogsCount = table.Column<int>(type: "integer", nullable: false),
                    GameStarted = table.Column<int>(type: "integer", nullable: false),
                    GameEnded = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerNotifications_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReplayGoals_PlayerId",
                table: "ReplayGoals",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerNotifications_PlayerId",
                table: "PlayerNotifications",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReplayGoals_Players_PlayerId",
                table: "ReplayGoals",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReplayGoals_Players_PlayerId",
                table: "ReplayGoals");

            migrationBuilder.DropTable(
                name: "PlayerNotifications");

            migrationBuilder.DropIndex(
                name: "IX_ReplayGoals_PlayerId",
                table: "ReplayGoals");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "ReplayGoals");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "ReplayGoals");

            migrationBuilder.AddColumn<List<string>>(
                name: "PushTokens",
                table: "Players",
                type: "text[]",
                nullable: false);
        }
    }
}
