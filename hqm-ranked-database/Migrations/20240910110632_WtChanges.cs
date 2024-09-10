using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_database.Migrations
{
    /// <inheritdoc />
    public partial class WtChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WeeklyTourneyId",
                table: "WeeklyTourneyGame",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyTourneyGame_WeeklyTourneyId",
                table: "WeeklyTourneyGame",
                column: "WeeklyTourneyId");

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyTourneyGame_WeeklyTourneys_WeeklyTourneyId",
                table: "WeeklyTourneyGame",
                column: "WeeklyTourneyId",
                principalTable: "WeeklyTourneys",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyTourneyGame_WeeklyTourneys_WeeklyTourneyId",
                table: "WeeklyTourneyGame");

            migrationBuilder.DropIndex(
                name: "IX_WeeklyTourneyGame_WeeklyTourneyId",
                table: "WeeklyTourneyGame");

            migrationBuilder.DropColumn(
                name: "WeeklyTourneyId",
                table: "WeeklyTourneyGame");
        }
    }
}
