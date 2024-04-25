using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedServerMax : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamMax",
                table: "Servers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamMax",
                table: "Servers");
        }
    }
}
