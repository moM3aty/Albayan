using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Albayan.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherToBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeacherId",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Books_TeacherId",
                table: "Books",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Teachers_TeacherId",
                table: "Books",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Teachers_TeacherId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_TeacherId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Books");
        }
    }
}
