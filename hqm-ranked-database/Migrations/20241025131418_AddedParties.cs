using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_database.Migrations
{
    /// <inheritdoc />
    public partial class AddedParties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeeklyTourneyParties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WeeklyTourneyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyTourneyParties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyTourneyParties_WeeklyTourneys_WeeklyTourneyId",
                        column: x => x.WeeklyTourneyId,
                        principalTable: "WeeklyTourneys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyTourneyPartyPlayers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WeeklyTourneyPartyId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyTourneyPartyPlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyTourneyPartyPlayers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeeklyTourneyPartyPlayers_WeeklyTourneyParties_WeeklyTourne~",
                        column: x => x.WeeklyTourneyPartyId,
                        principalTable: "WeeklyTourneyParties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyTourneyParties_WeeklyTourneyId",
                table: "WeeklyTourneyParties",
                column: "WeeklyTourneyId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyTourneyPartyPlayers_PlayerId",
                table: "WeeklyTourneyPartyPlayers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyTourneyPartyPlayers_WeeklyTourneyPartyId",
                table: "WeeklyTourneyPartyPlayers",
                column: "WeeklyTourneyPartyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeeklyTourneyPartyPlayers");

            migrationBuilder.DropTable(
                name: "WeeklyTourneyParties");
        }
    }
}
