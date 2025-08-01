using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication6.Migrations
{
    /// <inheritdoc />
    public partial class ayr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameCatagories_catagories_CatagoryId",
                table: "GameCatagories");

            migrationBuilder.DropForeignKey(
                name: "FK_GameCatagories_games_GameId",
                table: "GameCatagories");

            migrationBuilder.DropForeignKey(
                name: "FK_LibraryItems_games_GameId",
                table: "LibraryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_games_GameId",
                table: "OrderItems");

            migrationBuilder.AddForeignKey(
                name: "FK_GameCatagories_catagories_CatagoryId",
                table: "GameCatagories",
                column: "CatagoryId",
                principalTable: "catagories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GameCatagories_games_GameId",
                table: "GameCatagories",
                column: "GameId",
                principalTable: "games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LibraryItems_games_GameId",
                table: "LibraryItems",
                column: "GameId",
                principalTable: "games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_games_GameId",
                table: "OrderItems",
                column: "GameId",
                principalTable: "games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameCatagories_catagories_CatagoryId",
                table: "GameCatagories");

            migrationBuilder.DropForeignKey(
                name: "FK_GameCatagories_games_GameId",
                table: "GameCatagories");

            migrationBuilder.DropForeignKey(
                name: "FK_LibraryItems_games_GameId",
                table: "LibraryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_games_GameId",
                table: "OrderItems");

            migrationBuilder.AddForeignKey(
                name: "FK_GameCatagories_catagories_CatagoryId",
                table: "GameCatagories",
                column: "CatagoryId",
                principalTable: "catagories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameCatagories_games_GameId",
                table: "GameCatagories",
                column: "GameId",
                principalTable: "games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LibraryItems_games_GameId",
                table: "LibraryItems",
                column: "GameId",
                principalTable: "games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_games_GameId",
                table: "OrderItems",
                column: "GameId",
                principalTable: "games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
