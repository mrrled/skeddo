using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduleGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Classrooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ScheduleGroupId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classrooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Classrooms_ScheduleGroups_ScheduleGroupId",
                        column: x => x.ScheduleGroupId,
                        principalTable: "ScheduleGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ScheduleGroupId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedules_ScheduleGroups_ScheduleGroupId",
                        column: x => x.ScheduleGroupId,
                        principalTable: "ScheduleGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchoolSubjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ScheduleGroupId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolSubjects_ScheduleGroups_ScheduleGroupId",
                        column: x => x.ScheduleGroupId,
                        principalTable: "ScheduleGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ScheduleGroupId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Surname = table.Column<string>(type: "TEXT", nullable: false),
                    Patronymic = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teachers_ScheduleGroups_ScheduleGroupId",
                        column: x => x.ScheduleGroupId,
                        principalTable: "ScheduleGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonNumbers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ScheduleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Number = table.Column<int>(type: "INTEGER", nullable: false),
                    Time = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonNumbers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonNumbers_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ScheduleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyGroups_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchoolSubjectDboTeacherDbo",
                columns: table => new
                {
                    SchoolSubjectsId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TeachersId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolSubjectDboTeacherDbo", x => new { x.SchoolSubjectsId, x.TeachersId });
                    table.ForeignKey(
                        name: "FK_SchoolSubjectDboTeacherDbo_SchoolSubjects_SchoolSubjectsId",
                        column: x => x.SchoolSubjectsId,
                        principalTable: "SchoolSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchoolSubjectDboTeacherDbo_Teachers_TeachersId",
                        column: x => x.TeachersId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyGroupDboTeacherDbo",
                columns: table => new
                {
                    StudyGroupsId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TeachersId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyGroupDboTeacherDbo", x => new { x.StudyGroupsId, x.TeachersId });
                    table.ForeignKey(
                        name: "FK_StudyGroupDboTeacherDbo_StudyGroups_StudyGroupsId",
                        column: x => x.StudyGroupsId,
                        principalTable: "StudyGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyGroupDboTeacherDbo_Teachers_TeachersId",
                        column: x => x.TeachersId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudySubgroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    StudyGroupId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudySubgroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudySubgroups_StudyGroups_StudyGroupId",
                        column: x => x.StudyGroupId,
                        principalTable: "StudyGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonDrafts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LessonNumberId = table.Column<Guid>(type: "TEXT", nullable: true),
                    StudyGroupId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ClassroomId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SchoolSubjectId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TeacherId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ScheduleId = table.Column<Guid>(type: "TEXT", nullable: true),
                    StudySubgroupId = table.Column<Guid>(type: "TEXT", nullable: true)
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
                        name: "FK_LessonDrafts_StudySubgroups_StudySubgroupId",
                        column: x => x.StudySubgroupId,
                        principalTable: "StudySubgroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LessonDrafts_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LessonNumberId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StudyGroupId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClassroomId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SchoolSubjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TeacherId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ScheduleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StudySubgroupId = table.Column<Guid>(type: "TEXT", nullable: true),
                    WarningType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lessons_Classrooms_ClassroomId",
                        column: x => x.ClassroomId,
                        principalTable: "Classrooms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Lessons_LessonNumbers_LessonNumberId",
                        column: x => x.LessonNumberId,
                        principalTable: "LessonNumbers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Lessons_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Lessons_SchoolSubjects_SchoolSubjectId",
                        column: x => x.SchoolSubjectId,
                        principalTable: "SchoolSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Lessons_StudyGroups_StudyGroupId",
                        column: x => x.StudyGroupId,
                        principalTable: "StudyGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Lessons_StudySubgroups_StudySubgroupId",
                        column: x => x.StudySubgroupId,
                        principalTable: "StudySubgroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Lessons_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "ScheduleGroups",
                column: "Id",
                value: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Classrooms_ScheduleGroupId",
                table: "Classrooms",
                column: "ScheduleGroupId");

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
                name: "IX_LessonDrafts_StudySubgroupId",
                table: "LessonDrafts",
                column: "StudySubgroupId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonDrafts_TeacherId",
                table: "LessonDrafts",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonNumbers_ScheduleId",
                table: "LessonNumbers",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_ClassroomId",
                table: "Lessons",
                column: "ClassroomId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_LessonNumberId",
                table: "Lessons",
                column: "LessonNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_ScheduleId",
                table: "Lessons",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_SchoolSubjectId",
                table: "Lessons",
                column: "SchoolSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_StudyGroupId",
                table: "Lessons",
                column: "StudyGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_StudySubgroupId",
                table: "Lessons",
                column: "StudySubgroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_TeacherId",
                table: "Lessons",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_ScheduleGroupId",
                table: "Schedules",
                column: "ScheduleGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolSubjectDboTeacherDbo_TeachersId",
                table: "SchoolSubjectDboTeacherDbo",
                column: "TeachersId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolSubjects_ScheduleGroupId",
                table: "SchoolSubjects",
                column: "ScheduleGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyGroupDboTeacherDbo_TeachersId",
                table: "StudyGroupDboTeacherDbo",
                column: "TeachersId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyGroups_ScheduleId",
                table: "StudyGroups",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_StudySubgroups_StudyGroupId",
                table: "StudySubgroups",
                column: "StudyGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_ScheduleGroupId",
                table: "Teachers",
                column: "ScheduleGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LessonDrafts");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "SchoolSubjectDboTeacherDbo");

            migrationBuilder.DropTable(
                name: "StudyGroupDboTeacherDbo");

            migrationBuilder.DropTable(
                name: "Classrooms");

            migrationBuilder.DropTable(
                name: "LessonNumbers");

            migrationBuilder.DropTable(
                name: "StudySubgroups");

            migrationBuilder.DropTable(
                name: "SchoolSubjects");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "StudyGroups");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "ScheduleGroups");
        }
    }
}
