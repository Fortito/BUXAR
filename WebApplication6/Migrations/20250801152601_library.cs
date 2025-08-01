using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication6.Migrations
{
    /// <inheritdoc />
    public partial class library : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_games_GameId",
                table: "OrderItems");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_games_GameId",
                table: "OrderItems",
                column: "GameId",
                principalTable: "games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_games_GameId",
                table: "OrderItems");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_games_GameId",
                table: "OrderItems",
                column: "GameId",
                principalTable: "games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
