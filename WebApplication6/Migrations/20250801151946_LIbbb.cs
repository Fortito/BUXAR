using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication6.Migrations
{
    /// <inheritdoc />
    public partial class LIbbb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LibraryItems_Libraries_GameId",
                table: "LibraryItems");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryItems_LibraryId",
                table: "LibraryItems",
                column: "LibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_LibraryItems_Libraries_LibraryId",
                table: "LibraryItems",
                column: "LibraryId",
                principalTable: "Libraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LibraryItems_Libraries_LibraryId",
                table: "LibraryItems");

            migrationBuilder.DropIndex(
                name: "IX_LibraryItems_LibraryId",
                table: "LibraryItems");

            migrationBuilder.AddForeignKey(
                name: "FK_LibraryItems_Libraries_GameId",
                table: "LibraryItems",
                column: "GameId",
                principalTable: "Libraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
