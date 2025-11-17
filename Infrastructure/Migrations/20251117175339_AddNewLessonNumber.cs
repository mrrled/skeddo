using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewLessonNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Lessons_LessonNumberId",
                table: "Lessons",
                column: "LessonNumberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_LessonNumbers_LessonNumberId",
                table: "Lessons",
                column: "LessonNumberId",
                principalTable: "LessonNumbers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_LessonNumbers_LessonNumberId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_LessonNumberId",
                table: "Lessons");
        }
    }
}
