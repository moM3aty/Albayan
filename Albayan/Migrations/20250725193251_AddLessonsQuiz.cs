using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Albayan.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonsQuiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LessonQuizzes",
                columns: table => new
                {
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonQuizzes", x => x.LessonId);
                    table.ForeignKey(
                        name: "FK_LessonQuizzes_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LessonQuizAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LessonQuizId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    TotalPoints = table.Column<int>(type: "int", nullable: false),
                    AttemptDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonQuizAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonQuizAttempts_LessonQuizzes_LessonQuizId",
                        column: x => x.LessonQuizId,
                        principalTable: "LessonQuizzes",
                        principalColumn: "LessonId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LessonQuizAttempts_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LessonQuizQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OptionD = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrectAnswer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    LessonQuizId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonQuizQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonQuizQuestions_LessonQuizzes_LessonQuizId",
                        column: x => x.LessonQuizId,
                        principalTable: "LessonQuizzes",
                        principalColumn: "LessonId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LessonQuizAttemptAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LessonQuizAttemptId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    SelectedAnswer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonQuizAttemptAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonQuizAttemptAnswers_LessonQuizAttempts_LessonQuizAttemptId",
                        column: x => x.LessonQuizAttemptId,
                        principalTable: "LessonQuizAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LessonQuizAttemptAnswers_LessonQuizQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "LessonQuizQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonQuizAttemptAnswers_LessonQuizAttemptId",
                table: "LessonQuizAttemptAnswers",
                column: "LessonQuizAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonQuizAttemptAnswers_QuestionId",
                table: "LessonQuizAttemptAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonQuizAttempts_LessonQuizId",
                table: "LessonQuizAttempts",
                column: "LessonQuizId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonQuizAttempts_StudentId",
                table: "LessonQuizAttempts",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonQuizQuestions_LessonQuizId",
                table: "LessonQuizQuestions",
                column: "LessonQuizId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LessonQuizAttemptAnswers");

            migrationBuilder.DropTable(
                name: "LessonQuizAttempts");

            migrationBuilder.DropTable(
                name: "LessonQuizQuestions");

            migrationBuilder.DropTable(
                name: "LessonQuizzes");
        }
    }
}
