using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Albayan.Migrations
{
    /// <inheritdoc />
    public partial class AddBooksToTeacher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeacherId1",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_TeacherId1",
                table: "Books",
                column: "TeacherId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Teachers_TeacherId1",
                table: "Books",
                column: "TeacherId1",
                principalTable: "Teachers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Teachers_TeacherId1",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_TeacherId1",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "TeacherId1",
                table: "Books");
        }
    }
}
