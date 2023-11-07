using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedMissedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MvpId",
                table: "Games",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "GamePlayers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Games_MvpId",
                table: "Games",
                column: "MvpId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Players_MvpId",
                table: "Games",
                column: "MvpId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Players_MvpId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_MvpId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "MvpId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "GamePlayers");
        }
    }
}
