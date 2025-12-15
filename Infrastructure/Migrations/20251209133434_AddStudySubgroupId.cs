using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStudySubgroupId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudySubgroupId",
                table: "Lessons",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudySubgroupId",
                table: "LessonDrafts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_StudySubgroupId",
                table: "Lessons",
                column: "StudySubgroupId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonDrafts_StudySubgroupId",
                table: "LessonDrafts",
                column: "StudySubgroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonDrafts_StudySubgroups_StudySubgroupId",
                table: "LessonDrafts",
                column: "StudySubgroupId",
                principalTable: "StudySubgroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_StudySubgroups_StudySubgroupId",
                table: "Lessons",
                column: "StudySubgroupId",
                principalTable: "StudySubgroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonDrafts_StudySubgroups_StudySubgroupId",
                table: "LessonDrafts");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_StudySubgroups_StudySubgroupId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_StudySubgroupId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_LessonDrafts_StudySubgroupId",
                table: "LessonDrafts");

            migrationBuilder.DropColumn(
                name: "StudySubgroupId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "StudySubgroupId",
                table: "LessonDrafts");
        }
    }
}
