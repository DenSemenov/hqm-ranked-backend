using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedServerTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TokenId",
                table: "Servers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ServerTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    DivisionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServerTokens_Divisions_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Divisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Servers_TokenId",
                table: "Servers",
                column: "TokenId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerTokens_DivisionId",
                table: "ServerTokens",
                column: "DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Servers_ServerTokens_TokenId",
                table: "Servers",
                column: "TokenId",
                principalTable: "ServerTokens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
