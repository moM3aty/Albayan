using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Albayan.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStudentFromLessonsAttach : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonAttachments_Students_StudentId",
                table: "LessonAttachments");

            migrationBuilder.DropIndex(
                name: "IX_LessonAttachments_StudentId",
                table: "LessonAttachments");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "LessonAttachments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "LessonAttachments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LessonAttachments_StudentId",
                table: "LessonAttachments",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonAttachments_Students_StudentId",
                table: "LessonAttachments",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
