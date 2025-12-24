using Domain.Models;
using Infrastructure.DboExtensions;
using Infrastructure.DboModels;
using Xunit;
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Tests.RepositoryTests
{
    public class LessonDraftExtensionsTests
    {
        [Fact]
        public void ToLessonDraft_ValidDbo_ReturnsLessonDraft()
        {
            // Arrange
            var dbo = new LessonDraftDbo
            {
                Id = Guid.NewGuid(),
                ScheduleId = Guid.NewGuid(),
                SchoolSubject = new SchoolSubjectDbo { Id = Guid.NewGuid(), Name = "Math" },
                LessonNumber = new LessonNumberDbo { Number = 1, Time = "09:00" },
                Teacher = new TeacherDbo { Id = Guid.NewGuid(), Name = "John", Surname = "Doe", Patronymic = "Smith" },
                StudyGroup = new StudyGroupDbo { Id = Guid.NewGuid(), Name = "Group A" },
                Classroom = new ClassroomDbo { Id = Guid.NewGuid(), Name = "101" },
                StudySubgroup = new StudySubgroupDbo { Id = Guid.NewGuid(), Name = "Subgroup 1" }
            };

            // Act
            var result = dbo.ToLessonDraft();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dbo.Id, result.Id);
            Assert.Equal(dbo.ScheduleId, result.ScheduleId);
            Assert.Equal(dbo.SchoolSubject.Name, result.SchoolSubject.Name);
            Assert.Equal(dbo.LessonNumber.Number, result.LessonNumber.Number);
            Assert.Equal(dbo.Teacher.Name, result.Teacher.Name);
            Assert.Equal(dbo.StudyGroup.Name, result.StudyGroup.Name);
            Assert.Equal(dbo.Classroom.Name, result.Classroom.Name);
            Assert.Equal(dbo.StudySubgroup.Name, result.StudySubgroup.Name);
        }

        [Fact]
        public void ToLessonDraft_DboWithNullRelations_ReturnsLessonDraftWithNulls()
        {
            // Arrange
            var dbo = new LessonDraftDbo
            {
                Id = Guid.NewGuid(),
                ScheduleId = Guid.NewGuid(),
                SchoolSubject = new SchoolSubjectDbo { Id = Guid.NewGuid(), Name = "Math" }
                // Other relations are null
            };

            // Act
            var result = dbo.ToLessonDraft();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dbo.Id, result.Id);
            Assert.NotNull(result.SchoolSubject);
            Assert.Null(result.LessonNumber);
            Assert.Null(result.Teacher);
            Assert.Null(result.StudyGroup);
            Assert.Null(result.Classroom);
            Assert.Null(result.StudySubgroup);
        }

        [Fact]
        public void ToLessonDraftDbo_ValidLessonDraft_ReturnsDbo()
        {
            // Arrange
            var lessonDraftId = Guid.NewGuid();
            var scheduleId = Guid.NewGuid();
            var schoolSubject = SchoolSubject.CreateSchoolSubject(Guid.NewGuid(), "Math").Value;
            var lessonNumber = new LessonNumber(1, "09:00");
            var teacher = Teacher.CreateTeacher(Guid.NewGuid(), "John", "Doe", "Smith",
                [], []).Value;
            var studyGroup = StudyGroup.CreateStudyGroup(Guid.NewGuid(), scheduleId, "Group A").Value;
            var classroom = Classroom.CreateClassroom(Guid.NewGuid(), "101", null).Value;
            var studySubgroup = StudySubgroup.CreateStudySubgroup(studyGroup, "Subgroup 1").Value;

            var lessonDraft = new LessonDraft(
                lessonDraftId,
                scheduleId,
                schoolSubject,
                lessonNumber,
                teacher,
                studyGroup,
                classroom,
                studySubgroup,
                "Comment");

            // Act
            var result = lessonDraft.ToLessonDraftDbo();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(lessonDraftId, result.Id);
            Assert.Equal(scheduleId, result.ScheduleId);
            Assert.Equal(schoolSubject.Id, result.SchoolSubjectId);
        }

        [Fact]
        public void ToLessonDrafts_ValidDboList_ReturnsLessonDraftList()
        {
            // Arrange
            var dbos = new List<LessonDraftDbo>
            {
                new()
                { 
                    Id = Guid.NewGuid(), 
                    ScheduleId = Guid.NewGuid(),
                    SchoolSubject = new SchoolSubjectDbo { Id = Guid.NewGuid(), Name = "Math" }
                },
                new()
                { 
                    Id = Guid.NewGuid(), 
                    ScheduleId = Guid.NewGuid(),
                    SchoolSubject = new SchoolSubjectDbo { Id = Guid.NewGuid(), Name = "Physics" }
                }
            };

            // Act
            var result = dbos.ToLessonDrafts();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, Assert.NotNull);
        }

        [Fact]
        public void ToLessonDrafts_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            var emptyList = new List<LessonDraftDbo>();

            // Act
            var result = emptyList.ToLessonDrafts();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void ToLessonDrafts_NullList_ReturnsEmptyList()
        {
            // Arrange
            List<LessonDraftDbo> nullList = null;

            // Act
            var result = nullList.ToLessonDrafts();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}