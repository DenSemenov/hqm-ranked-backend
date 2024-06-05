using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedReportFromWeb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GameId",
                table: "Reports",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ReasonId",
                table: "Reports",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Tick",
                table: "Reports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_GameId",
                table: "Reports",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReasonId",
                table: "Reports",
                column: "ReasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Games_GameId",
                table: "Reports",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Rules_ReasonId",
                table: "Reports",
                column: "ReasonId",
                principalTable: "Rules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Games_GameId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Rules_ReasonId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_GameId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ReasonId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ReasonId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "Tick",
                table: "Reports");
        }
    }
}
