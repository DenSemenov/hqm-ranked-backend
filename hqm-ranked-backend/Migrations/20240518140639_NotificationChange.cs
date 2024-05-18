using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_backend.Migrations
{
    /// <inheritdoc />
    public partial class NotificationChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PushTokens",
                table: "Players");

            migrationBuilder.AddColumn<Guid>(
                name: "NotificationsId",
                table: "Players",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PlayerNotificationSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_PlayerNotificationSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_NotificationsId",
                table: "Players",
                column: "NotificationsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_PlayerNotificationSettings_NotificationsId",
                table: "Players",
                column: "NotificationsId",
                principalTable: "PlayerNotificationSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_PlayerNotificationSettings_NotificationsId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "PlayerNotificationSettings");

            migrationBuilder.DropIndex(
                name: "IX_Players_NotificationsId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "NotificationsId",
                table: "Players");

            migrationBuilder.AddColumn<List<string>>(
                name: "PushTokens",
                table: "Players",
                type: "text[]",
                nullable: false);
        }
    }
}
