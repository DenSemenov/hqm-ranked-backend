using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_database.Migrations
{
    /// <inheritdoc />
    public partial class AddedNextGameToWT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "NextGameId",
                table: "WeeklyTourneyGame",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyTourneyGame_NextGameId",
                table: "WeeklyTourneyGame",
                column: "NextGameId");

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyTourneyGame_WeeklyTourneyGame_NextGameId",
                table: "WeeklyTourneyGame",
                column: "NextGameId",
                principalTable: "WeeklyTourneyGame",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyTourneyGame_WeeklyTourneyGame_NextGameId",
                table: "WeeklyTourneyGame");

            migrationBuilder.DropIndex(
                name: "IX_WeeklyTourneyGame_NextGameId",
                table: "WeeklyTourneyGame");

            migrationBuilder.DropColumn(
                name: "NextGameId",
                table: "WeeklyTourneyGame");
        }
    }
}
