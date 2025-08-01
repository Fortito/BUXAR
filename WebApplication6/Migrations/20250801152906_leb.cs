using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication6.Migrations
{
    /// <inheritdoc />
    public partial class leb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LibraryItems_games_GameId",
                table: "LibraryItems");

            migrationBuilder.AddForeignKey(
                name: "FK_LibraryItems_games_GameId",
                table: "LibraryItems",
                column: "GameId",
                principalTable: "games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LibraryItems_games_GameId",
                table: "LibraryItems");

            migrationBuilder.AddForeignKey(
                name: "FK_LibraryItems_games_GameId",
                table: "LibraryItems",
                column: "GameId",
                principalTable: "games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
