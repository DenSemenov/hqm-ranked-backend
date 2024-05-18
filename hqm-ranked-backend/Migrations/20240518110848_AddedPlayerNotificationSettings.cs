using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedPlayerNotificationSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "NotificationSettingId",
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
                    GameStarted = table.Column<bool>(type: "boolean", nullable: false),
                    GameStartedWithMe = table.Column<bool>(type: "boolean", nullable: false),
                    GameEnded = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerNotificationSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_NotificationSettingId",
                table: "Players",
                column: "NotificationSettingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_PlayerNotificationSettings_NotificationSettingId",
                table: "Players",
                column: "NotificationSettingId",
                principalTable: "PlayerNotificationSettings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_PlayerNotificationSettings_NotificationSettingId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "PlayerNotificationSettings");

            migrationBuilder.DropIndex(
                name: "IX_Players_NotificationSettingId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "NotificationSettingId",
                table: "Players");
        }
    }
}
