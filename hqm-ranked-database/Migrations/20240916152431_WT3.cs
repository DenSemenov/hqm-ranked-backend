using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_database.Migrations
{
    /// <inheritdoc />
    public partial class WT3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyTourneyGame_Games_GameId",
                table: "WeeklyTourneyGame");

            migrationBuilder.AlterColumn<Guid>(
                name: "GameId",
                table: "WeeklyTourneyGame",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyTourneyGame_Games_GameId",
                table: "WeeklyTourneyGame",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyTourneyGame_Games_GameId",
                table: "WeeklyTourneyGame");

            migrationBuilder.AlterColumn<Guid>(
                name: "GameId",
                table: "WeeklyTourneyGame",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyTourneyGame_Games_GameId",
                table: "WeeklyTourneyGame",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
