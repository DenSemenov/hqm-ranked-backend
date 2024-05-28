using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedStoriesLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReplayGoalId",
                table: "Players",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_ReplayGoalId",
                table: "Players",
                column: "ReplayGoalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_ReplayGoals_ReplayGoalId",
                table: "Players",
                column: "ReplayGoalId",
                principalTable: "ReplayGoals",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_ReplayGoals_ReplayGoalId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_ReplayGoalId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "ReplayGoalId",
                table: "Players");
        }
    }
}
