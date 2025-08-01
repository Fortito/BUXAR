using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication6.Migrations
{
    /// <inheritdoc />
    public partial class category : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Libraries_LibraryId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_LibraryId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "LibraryId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "LibraryItems");

            migrationBuilder.AddColumn<string>(
                name: "CatagoryName",
                table: "LibraryItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CatagoryName",
                table: "LibraryItems");

            migrationBuilder.AddColumn<int>(
                name: "LibraryId",
                table: "OrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "LibraryItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_LibraryId",
                table: "OrderItems",
                column: "LibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Libraries_LibraryId",
                table: "OrderItems",
                column: "LibraryId",
                principalTable: "Libraries",
                principalColumn: "Id");
        }
    }
}
