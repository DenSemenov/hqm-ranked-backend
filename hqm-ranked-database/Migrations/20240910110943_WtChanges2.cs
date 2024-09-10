using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_database.Migrations
{
    /// <inheritdoc />
    public partial class WtChanges2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyTourneyGame_WeeklyTourneys_WeeklyTourneyId",
                table: "WeeklyTourneyGame");

            migrationBuilder.AlterColumn<Guid>(
                name: "WeeklyTourneyId",
                table: "WeeklyTourneyGame",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyTourneyGame_WeeklyTourneys_WeeklyTourneyId",
                table: "WeeklyTourneyGame",
                column: "WeeklyTourneyId",
                principalTable: "WeeklyTourneys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyTourneyGame_WeeklyTourneys_WeeklyTourneyId",
                table: "WeeklyTourneyGame");

            migrationBuilder.AlterColumn<Guid>(
                name: "WeeklyTourneyId",
                table: "WeeklyTourneyGame",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyTourneyGame_WeeklyTourneys_WeeklyTourneyId",
                table: "WeeklyTourneyGame",
                column: "WeeklyTourneyId",
                principalTable: "WeeklyTourneys",
                principalColumn: "Id");
        }
    }
}
