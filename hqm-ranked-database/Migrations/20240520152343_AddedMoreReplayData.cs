using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedMoreReplayData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReplayChats_ReplayData_ReplayDataId",
                table: "ReplayChats");

            migrationBuilder.DropForeignKey(
                name: "FK_ReplayGoals_ReplayData_ReplayDataId",
                table: "ReplayGoals");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReplayDataId",
                table: "ReplayGoals",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ReplayDataId",
                table: "ReplayChats",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Conceded",
                table: "GamePlayers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Possession",
                table: "GamePlayers",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Saves",
                table: "GamePlayers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Shots",
                table: "GamePlayers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ReplayHighlight",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Packet = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Period = table.Column<int>(type: "integer", nullable: false),
                    Time = table.Column<int>(type: "integer", nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    ReplayDataId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplayHighlight", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReplayHighlight_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReplayHighlight_ReplayData_ReplayDataId",
                        column: x => x.ReplayDataId,
                        principalTable: "ReplayData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReplayHighlight_PlayerId",
                table: "ReplayHighlight",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReplayHighlight_ReplayDataId",
                table: "ReplayHighlight",
                column: "ReplayDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReplayChats_ReplayData_ReplayDataId",
                table: "ReplayChats",
                column: "ReplayDataId",
                principalTable: "ReplayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReplayGoals_ReplayData_ReplayDataId",
                table: "ReplayGoals",
                column: "ReplayDataId",
                principalTable: "ReplayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReplayChats_ReplayData_ReplayDataId",
                table: "ReplayChats");

            migrationBuilder.DropForeignKey(
                name: "FK_ReplayGoals_ReplayData_ReplayDataId",
                table: "ReplayGoals");

            migrationBuilder.DropTable(
                name: "ReplayHighlight");

            migrationBuilder.DropColumn(
                name: "Conceded",
                table: "GamePlayers");

            migrationBuilder.DropColumn(
                name: "Possession",
                table: "GamePlayers");

            migrationBuilder.DropColumn(
                name: "Saves",
                table: "GamePlayers");

            migrationBuilder.DropColumn(
                name: "Shots",
                table: "GamePlayers");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReplayDataId",
                table: "ReplayGoals",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReplayDataId",
                table: "ReplayChats",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_ReplayChats_ReplayData_ReplayDataId",
                table: "ReplayChats",
                column: "ReplayDataId",
                principalTable: "ReplayData",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReplayGoals_ReplayData_ReplayDataId",
                table: "ReplayGoals",
                column: "ReplayDataId",
                principalTable: "ReplayData",
                principalColumn: "Id");
        }
    }
}
