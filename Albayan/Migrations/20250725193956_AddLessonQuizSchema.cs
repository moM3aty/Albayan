using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Albayan.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonQuizSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonQuizAttempts_Students_StudentId",
                table: "LessonQuizAttempts");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonQuizAttempts_Students_StudentId",
                table: "LessonQuizAttempts",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonQuizAttempts_Students_StudentId",
                table: "LessonQuizAttempts");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonQuizAttempts_Students_StudentId",
                table: "LessonQuizAttempts",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
