using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Albayan.Migrations
{
    /// <inheritdoc />
    public partial class EditPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VideoUrl",
                table: "Lessons",
                newName: "BunnyVideoLibraryId");

            migrationBuilder.AddColumn<string>(
                name: "BunnyVideoId",
                table: "Lessons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BunnyVideoId",
                table: "Lessons");

            migrationBuilder.RenameColumn(
                name: "BunnyVideoLibraryId",
                table: "Lessons",
                newName: "VideoUrl");
        }
    }
}
