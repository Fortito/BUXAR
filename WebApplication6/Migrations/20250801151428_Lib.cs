using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication6.Migrations
{
    /// <inheritdoc />
    public partial class Lib : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LibraryItems_Libraries_LibraryId",
                table: "LibraryItems");

            migrationBuilder.DropTable(
                name: "contacts");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LibraryItems_Libraries_GameId",
                table: "LibraryItems");

            migrationBuilder.CreateTable(
                name: "contacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mesage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sernume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contacts", x => x.Id);
                });

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
    }
}
