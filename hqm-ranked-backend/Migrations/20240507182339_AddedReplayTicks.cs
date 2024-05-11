using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedReplayTicks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Ticks",
                table: "ReplayData",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ReplayFragments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTick = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false),
                    ReplayDataId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplayFragments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReplayFragments_ReplayData_ReplayDataId",
                        column: x => x.ReplayDataId,
                        principalTable: "ReplayData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReplayFragments_ReplayDataId",
                table: "ReplayFragments",
                column: "ReplayDataId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReplayFragments");

            migrationBuilder.DropColumn(
                name: "Ticks",
                table: "ReplayData");
        }
    }
}
