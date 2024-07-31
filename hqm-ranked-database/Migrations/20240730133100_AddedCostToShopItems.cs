using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_database.Migrations
{
    /// <inheritdoc />
    public partial class AddedCostToShopItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Cost",
                table: "ShopItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cost",
                table: "ShopItems");
        }
    }
}
