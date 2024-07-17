using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_database.Migrations
{
    /// <inheritdoc />
    public partial class AddedLimitsType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LimitType",
                table: "Players",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LimitType",
                table: "Players");
        }
    }
}
