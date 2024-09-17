using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_database.Migrations
{
    /// <inheritdoc />
    public partial class WTTeamNotRequeired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyTourneyGame_WeeklyTourneyTeams_BlueTeamId",
                table: "WeeklyTourneyGame");

            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyTourneyGame_WeeklyTourneyTeams_RedTeamId",
                table: "WeeklyTourneyGame");

            migrationBuilder.AlterColumn<Guid>(
                name: "RedTeamId",
                table: "WeeklyTourneyGame",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "BlueTeamId",
                table: "WeeklyTourneyGame",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyTourneyGame_WeeklyTourneyTeams_BlueTeamId",
                table: "WeeklyTourneyGame",
                column: "BlueTeamId",
                principalTable: "WeeklyTourneyTeams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyTourneyGame_WeeklyTourneyTeams_RedTeamId",
                table: "WeeklyTourneyGame",
                column: "RedTeamId",
                principalTable: "WeeklyTourneyTeams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyTourneyGame_WeeklyTourneyTeams_BlueTeamId",
                table: "WeeklyTourneyGame");

            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyTourneyGame_WeeklyTourneyTeams_RedTeamId",
                table: "WeeklyTourneyGame");

            migrationBuilder.AlterColumn<Guid>(
                name: "RedTeamId",
                table: "WeeklyTourneyGame",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "BlueTeamId",
                table: "WeeklyTourneyGame",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyTourneyGame_WeeklyTourneyTeams_BlueTeamId",
                table: "WeeklyTourneyGame",
                column: "BlueTeamId",
                principalTable: "WeeklyTourneyTeams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyTourneyGame_WeeklyTourneyTeams_RedTeamId",
                table: "WeeklyTourneyGame",
                column: "RedTeamId",
                principalTable: "WeeklyTourneyTeams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
