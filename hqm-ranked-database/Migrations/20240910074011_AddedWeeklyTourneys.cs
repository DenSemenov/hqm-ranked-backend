using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_database.Migrations
{
    /// <inheritdoc />
    public partial class AddedWeeklyTourneys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeeklyTourneys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyTourneys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyTourneyRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WeeklyTourneyId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    Positions = table.Column<int[]>(type: "integer[]", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyTourneyRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyTourneyRequests_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeeklyTourneyRequests_WeeklyTourneys_WeeklyTourneyId",
                        column: x => x.WeeklyTourneyId,
                        principalTable: "WeeklyTourneys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyTourneyTeams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WeeklyTourneyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyTourneyTeams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyTourneyTeams_WeeklyTourneys_WeeklyTourneyId",
                        column: x => x.WeeklyTourneyId,
                        principalTable: "WeeklyTourneys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyTourneyGame",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    RedTeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    BlueTeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayoffType = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyTourneyGame", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyTourneyGame_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeeklyTourneyGame_WeeklyTourneyTeams_BlueTeamId",
                        column: x => x.BlueTeamId,
                        principalTable: "WeeklyTourneyTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeeklyTourneyGame_WeeklyTourneyTeams_RedTeamId",
                        column: x => x.RedTeamId,
                        principalTable: "WeeklyTourneyTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyTourneyPlayers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyTourneyPlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyTourneyPlayers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeeklyTourneyPlayers_WeeklyTourneyTeams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "WeeklyTourneyTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyTourneyGame_BlueTeamId",
                table: "WeeklyTourneyGame",
                column: "BlueTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyTourneyGame_GameId",
                table: "WeeklyTourneyGame",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyTourneyGame_RedTeamId",
                table: "WeeklyTourneyGame",
                column: "RedTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyTourneyPlayers_PlayerId",
                table: "WeeklyTourneyPlayers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyTourneyPlayers_TeamId",
                table: "WeeklyTourneyPlayers",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyTourneyRequests_PlayerId",
                table: "WeeklyTourneyRequests",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyTourneyRequests_WeeklyTourneyId",
                table: "WeeklyTourneyRequests",
                column: "WeeklyTourneyId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyTourneyTeams_WeeklyTourneyId",
                table: "WeeklyTourneyTeams",
                column: "WeeklyTourneyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeeklyTourneyGame");

            migrationBuilder.DropTable(
                name: "WeeklyTourneyPlayers");

            migrationBuilder.DropTable(
                name: "WeeklyTourneyRequests");

            migrationBuilder.DropTable(
                name: "WeeklyTourneyTeams");

            migrationBuilder.DropTable(
                name: "WeeklyTourneys");
        }
    }
}
