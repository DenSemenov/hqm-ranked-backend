using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_backend.Migrations
{
    /// <inheritdoc />
    public partial class RemovedServerTokenEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servers_ServerTokens_TokenId",
                table: "Servers");

            migrationBuilder.DropTable(
                name: "ServerTokens");

            migrationBuilder.DropIndex(
                name: "IX_Servers_TokenId",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "TokenId",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "Seasons");

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "Servers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Token",
                table: "Servers");

            migrationBuilder.AddColumn<Guid>(
                name: "TokenId",
                table: "Servers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DivisionId",
                table: "Seasons",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ServerTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Token = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerTokens", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Servers_TokenId",
                table: "Servers",
                column: "TokenId");

            migrationBuilder.AddForeignKey(
                name: "FK_Servers_ServerTokens_TokenId",
                table: "Servers",
                column: "TokenId",
                principalTable: "ServerTokens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
