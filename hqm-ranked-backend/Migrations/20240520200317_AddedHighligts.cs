using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedHighligts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReplayHighlight_Players_PlayerId",
                table: "ReplayHighlight");

            migrationBuilder.DropForeignKey(
                name: "FK_ReplayHighlight_ReplayData_ReplayDataId",
                table: "ReplayHighlight");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReplayHighlight",
                table: "ReplayHighlight");

            migrationBuilder.RenameTable(
                name: "ReplayHighlight",
                newName: "ReplayHighlights");

            migrationBuilder.RenameIndex(
                name: "IX_ReplayHighlight_ReplayDataId",
                table: "ReplayHighlights",
                newName: "IX_ReplayHighlights_ReplayDataId");

            migrationBuilder.RenameIndex(
                name: "IX_ReplayHighlight_PlayerId",
                table: "ReplayHighlights",
                newName: "IX_ReplayHighlights_PlayerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReplayHighlights",
                table: "ReplayHighlights",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReplayHighlights_Players_PlayerId",
                table: "ReplayHighlights",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReplayHighlights_ReplayData_ReplayDataId",
                table: "ReplayHighlights",
                column: "ReplayDataId",
                principalTable: "ReplayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReplayHighlights_Players_PlayerId",
                table: "ReplayHighlights");

            migrationBuilder.DropForeignKey(
                name: "FK_ReplayHighlights_ReplayData_ReplayDataId",
                table: "ReplayHighlights");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReplayHighlights",
                table: "ReplayHighlights");

            migrationBuilder.RenameTable(
                name: "ReplayHighlights",
                newName: "ReplayHighlight");

            migrationBuilder.RenameIndex(
                name: "IX_ReplayHighlights_ReplayDataId",
                table: "ReplayHighlight",
                newName: "IX_ReplayHighlight_ReplayDataId");

            migrationBuilder.RenameIndex(
                name: "IX_ReplayHighlights_PlayerId",
                table: "ReplayHighlight",
                newName: "IX_ReplayHighlight_PlayerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReplayHighlight",
                table: "ReplayHighlight",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReplayHighlight_Players_PlayerId",
                table: "ReplayHighlight",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReplayHighlight_ReplayData_ReplayDataId",
                table: "ReplayHighlight",
                column: "ReplayDataId",
                principalTable: "ReplayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
