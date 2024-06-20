using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedMusicToGoal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MusicId",
                table: "ReplayGoals",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReplayGoals_MusicId",
                table: "ReplayGoals",
                column: "MusicId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReplayGoals_Music_MusicId",
                table: "ReplayGoals",
                column: "MusicId",
                principalTable: "Music",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReplayGoals_Music_MusicId",
                table: "ReplayGoals");

            migrationBuilder.DropIndex(
                name: "IX_ReplayGoals_MusicId",
                table: "ReplayGoals");

            migrationBuilder.DropColumn(
                name: "MusicId",
                table: "ReplayGoals");
        }
    }
}
