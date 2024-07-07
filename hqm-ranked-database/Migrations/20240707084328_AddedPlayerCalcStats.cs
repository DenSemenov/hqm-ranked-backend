using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_database.Migrations
{
    /// <inheritdoc />
    public partial class AddedPlayerCalcStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PlayerCalcStatsId",
                table: "Players",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PlayerCalcStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Mvp = table.Column<double>(type: "double precision", nullable: false),
                    Winrate = table.Column<double>(type: "double precision", nullable: false),
                    Goals = table.Column<double>(type: "double precision", nullable: false),
                    Assists = table.Column<double>(type: "double precision", nullable: false),
                    Shots = table.Column<double>(type: "double precision", nullable: false),
                    Saves = table.Column<double>(type: "double precision", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCalcStats", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_PlayerCalcStatsId",
                table: "Players",
                column: "PlayerCalcStatsId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_PlayerCalcStats_PlayerCalcStatsId",
                table: "Players",
                column: "PlayerCalcStatsId",
                principalTable: "PlayerCalcStats",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_PlayerCalcStats_PlayerCalcStatsId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "PlayerCalcStats");

            migrationBuilder.DropIndex(
                name: "IX_Players_PlayerCalcStatsId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PlayerCalcStatsId",
                table: "Players");
        }
    }
}
