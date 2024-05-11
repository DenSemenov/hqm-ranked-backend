using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_backend.Migrations
{
    /// <inheritdoc />
    public partial class ChangedReplayFragmentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartTick",
                table: "ReplayFragments");

            migrationBuilder.DropColumn(
                name: "Ticks",
                table: "ReplayData");

            migrationBuilder.RenameColumn(
                name: "StartTick",
                table: "ReplayData",
                newName: "Min");

            migrationBuilder.AddColumn<int>(
                name: "Index",
                table: "ReplayFragments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "Max",
                table: "ReplayData",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Index",
                table: "ReplayFragments");

            migrationBuilder.DropColumn(
                name: "Max",
                table: "ReplayData");

            migrationBuilder.RenameColumn(
                name: "Min",
                table: "ReplayData",
                newName: "StartTick");

            migrationBuilder.AddColumn<long>(
                name: "StartTick",
                table: "ReplayFragments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "Ticks",
                table: "ReplayData",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
