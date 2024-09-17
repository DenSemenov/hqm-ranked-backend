using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_database.Migrations
{
    /// <inheritdoc />
    public partial class WTServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ServerId",
                table: "WeeklyTourneyGame",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyTourneyGame_ServerId",
                table: "WeeklyTourneyGame",
                column: "ServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyTourneyGame_Servers_ServerId",
                table: "WeeklyTourneyGame",
                column: "ServerId",
                principalTable: "Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyTourneyGame_Servers_ServerId",
                table: "WeeklyTourneyGame");

            migrationBuilder.DropIndex(
                name: "IX_WeeklyTourneyGame_ServerId",
                table: "WeeklyTourneyGame");

            migrationBuilder.DropColumn(
                name: "ServerId",
                table: "WeeklyTourneyGame");
        }
    }
}
