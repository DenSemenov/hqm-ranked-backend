using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedDivisions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DivisionId",
                table: "Seasons",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Divisions",
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
                    table.PrimaryKey("PK_Divisions", x => x.Id);
                });

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

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seasons_Divisions_DivisionId",
                table: "Seasons");

            migrationBuilder.DropTable(
                name: "Divisions");

            migrationBuilder.DropIndex(
                name: "IX_Seasons_DivisionId",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "Seasons");
        }
    }
}
