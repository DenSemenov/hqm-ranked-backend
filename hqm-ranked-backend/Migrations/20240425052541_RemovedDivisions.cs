using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_backend.Migrations
{
    /// <inheritdoc />
    public partial class RemovedDivisions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seasons_Divisions_DivisionId",
                table: "Seasons");

            migrationBuilder.DropForeignKey(
                name: "FK_ServerTokens_Divisions_DivisionId",
                table: "ServerTokens");

            migrationBuilder.DropTable(
                name: "Divisions");

            migrationBuilder.DropIndex(
                name: "IX_ServerTokens_DivisionId",
                table: "ServerTokens");

            migrationBuilder.DropIndex(
                name: "IX_Seasons_DivisionId",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "ServerTokens");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DivisionId",
                table: "ServerTokens",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Divisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Divisions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServerTokens_DivisionId",
                table: "ServerTokens",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_DivisionId",
                table: "Seasons",
                column: "DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Seasons_Divisions_DivisionId",
                table: "Seasons",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServerTokens_Divisions_DivisionId",
                table: "ServerTokens",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
