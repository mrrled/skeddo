using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonDraft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LessonDrafts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LessonNumberId = table.Column<int>(type: "INTEGER", nullable: true),
                    StudyGroupId = table.Column<int>(type: "INTEGER", nullable: true),
                    ClassroomId = table.Column<int>(type: "INTEGER", nullable: true),
                    SchoolSubjectId = table.Column<int>(type: "INTEGER", nullable: true),
                    TeacherId = table.Column<int>(type: "INTEGER", nullable: true),
                    ScheduleId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonDrafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonDrafts_Classrooms_ClassroomId",
                        column: x => x.ClassroomId,
                        principalTable: "Classrooms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LessonDrafts_LessonNumbers_LessonNumberId",
                        column: x => x.LessonNumberId,
                        principalTable: "LessonNumbers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LessonDrafts_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LessonDrafts_SchoolSubjects_SchoolSubjectId",
                        column: x => x.SchoolSubjectId,
                        principalTable: "SchoolSubjects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LessonDrafts_StudyGroups_StudyGroupId",
                        column: x => x.StudyGroupId,
                        principalTable: "StudyGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LessonDrafts_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonDrafts_ClassroomId",
                table: "LessonDrafts",
                column: "ClassroomId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonDrafts_LessonNumberId",
                table: "LessonDrafts",
                column: "LessonNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonDrafts_ScheduleId",
                table: "LessonDrafts",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonDrafts_SchoolSubjectId",
                table: "LessonDrafts",
                column: "SchoolSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonDrafts_StudyGroupId",
                table: "LessonDrafts",
                column: "StudyGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonDrafts_TeacherId",
                table: "LessonDrafts",
                column: "TeacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LessonDrafts");
        }
    }
}
