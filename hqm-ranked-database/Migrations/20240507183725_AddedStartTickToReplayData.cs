using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hqm_ranked_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedStartTickToReplayData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReplayFragments_ReplayData_ReplayDataId",
                table: "ReplayFragments");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReplayDataId",
                table: "ReplayFragments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StartTick",
                table: "ReplayData",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddForeignKey(
                name: "FK_ReplayFragments_ReplayData_ReplayDataId",
                table: "ReplayFragments",
                column: "ReplayDataId",
                principalTable: "ReplayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReplayFragments_ReplayData_ReplayDataId",
                table: "ReplayFragments");

            migrationBuilder.DropColumn(
                name: "StartTick",
                table: "ReplayData");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReplayDataId",
                table: "ReplayFragments",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_ReplayFragments_ReplayData_ReplayDataId",
                table: "ReplayFragments",
                column: "ReplayDataId",
                principalTable: "ReplayData",
                principalColumn: "Id");
        }
    }
}
