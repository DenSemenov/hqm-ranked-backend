using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_backend.Migrations
{
    /// <inheritdoc />
    public partial class MovedToS3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "ReplayData");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "ReplayData",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "ReplayData");

            migrationBuilder.AddColumn<byte[]>(
                name: "Data",
                table: "ReplayData",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
