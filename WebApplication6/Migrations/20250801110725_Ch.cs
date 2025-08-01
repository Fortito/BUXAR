using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication6.Migrations
{
    /// <inheritdoc />
    public partial class Ch : Migration
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
        }
    }
}
