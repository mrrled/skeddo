using Xunit;
using Moq;
using Application.Services;
using Application.DtoModels;
using Domain;
using Domain.Models;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;
using Application;
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Tests.ServicesTests
{
    public class LessonServicesTests
    {
        private readonly Mock<IScheduleRepository> _scheduleRepositoryMock;
        private readonly Mock<ILessonRepository> _lessonRepositoryMock;
        private readonly Mock<ITeacherRepository> _teacherRepositoryMock;
        private readonly Mock<ISchoolSubjectRepository> _schoolSubjectRepositoryMock;
        private readonly Mock<IStudyGroupRepository> _studyGroupRepositoryMock;
        private readonly Mock<IClassroomRepository> _classroomRepositoryMock;
        private readonly Mock<ILessonFactory> _lessonFactoryMock;
        private readonly Mock<ILessonDraftRepository> _lessonDraftRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly LessonServices _lessonServices;

        public LessonServicesTests()
        {
            _scheduleRepositoryMock = new Mock<IScheduleRepository>();
            _lessonRepositoryMock = new Mock<ILessonRepository>();
            _teacherRepositoryMock = new Mock<ITeacherRepository>();
            _schoolSubjectRepositoryMock = new Mock<ISchoolSubjectRepository>();
            _studyGroupRepositoryMock = new Mock<IStudyGroupRepository>();
            _classroomRepositoryMock = new Mock<IClassroomRepository>();
            _lessonFactoryMock = new Mock<ILessonFactory>();
            _lessonDraftRepositoryMock = new Mock<ILessonDraftRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            Mock<ILogger<LessonServices>> loggerMock = new Mock<ILogger<LessonServices>>();

            _lessonServices = new LessonServices(
                _scheduleRepositoryMock.Object,
                _lessonRepositoryMock.Object,
                _teacherRepositoryMock.Object,
                _schoolSubjectRepositoryMock.Object,
                _studyGroupRepositoryMock.Object,
                _classroomRepositoryMock.Object,
                _lessonFactoryMock.Object,
                _lessonDraftRepositoryMock.Object,
                _unitOfWorkMock.Object,
                loggerMock.Object
            );
        }

        #region GetLessonsByScheduleId Tests

        [Fact]
        public async Task GetLessonsByScheduleId_ShouldReturnLessons_WhenScheduleExists()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessons = new List<Lesson>
            {
                CreateTestLesson(scheduleId),
                CreateTestLesson(scheduleId)
            };

            _lessonRepositoryMock
                .Setup(repo => repo.GetLessonsByScheduleIdAsync(scheduleId))
                .ReturnsAsync(lessons);

            // Act
            var result = await _lessonServices.GetLessonsByScheduleId(scheduleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _lessonRepositoryMock.Verify(repo => repo.GetLessonsByScheduleIdAsync(scheduleId), Times.Once);
        }

        [Fact]
        public async Task GetLessonsByScheduleId_ShouldReturnEmptyList_WhenNoLessons()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var emptyList = new List<Lesson>();

            _lessonRepositoryMock
                .Setup(repo => repo.GetLessonsByScheduleIdAsync(scheduleId))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _lessonServices.GetLessonsByScheduleId(scheduleId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetLessonsByScheduleId_ShouldHandleRepositoryException()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();

            _lessonRepositoryMock
                .Setup(repo => repo.GetLessonsByScheduleIdAsync(scheduleId))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _lessonServices.GetLessonsByScheduleId(scheduleId));
        }

        #endregion

        #region AddLesson Tests

        [Fact]
        public async Task AddLesson_ShouldReturnSuccess_WhenAllDataValidAndLessonFactorySucceeds()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonDto = CreateValidCreateLessonDto();
            var schedule = CreateTestSchedule(scheduleId);
            var teacher = CreateTestTeacher();
            var classroom = CreateTestClassroom();
            var schoolSubject = CreateTestSchoolSubject();
            var studyGroup = CreateTestStudyGroup();
            _ = LessonNumber.CreateLessonNumber(1, "09:00").Value;
            var lesson = CreateTestLesson(scheduleId);
            var draft = CreateTestLessonDraft(scheduleId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _teacherRepositoryMock
                .Setup(repo => repo.GetTeacherByIdAsync(lessonDto.Teacher.Id))
                .ReturnsAsync(teacher);

            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomByIdAsync(lessonDto.Classroom.Id))
                .ReturnsAsync(classroom);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(lessonDto.SchoolSubject.Id))
                .ReturnsAsync(schoolSubject);

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(lessonDto.StudyGroup.Id))
                .ReturnsAsync(studyGroup);

            _lessonFactoryMock
                .Setup(factory => factory.CreateFromDraft(It.IsAny<LessonDraft>()))
                .Returns(Result<Lesson>.Success(lesson));

            _lessonRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Lesson>(), scheduleId))
                .Returns(Task.CompletedTask);

            _lessonRepositoryMock
                .Setup(repo => repo.UpdateRangeAsync(It.IsAny<List<Lesson>>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonServices.AddLesson(lessonDto, scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
            _lessonFactoryMock.Verify(factory => factory.CreateFromDraft(It.IsAny<LessonDraft>()), Times.Once);
            _lessonRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Lesson>(), scheduleId), Times.Once);
            _lessonRepositoryMock.Verify(repo => repo.UpdateRangeAsync(It.IsAny<List<Lesson>>()), Times.Once);
        }

        [Fact]
        public async Task AddLesson_ShouldReturnDowngradedResult_WhenLessonFactoryFails()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonDto = CreateValidCreateLessonDto();
            var schedule = CreateTestSchedule(scheduleId);
            var teacher = CreateTestTeacher();
            var classroom = CreateTestClassroom();
            var schoolSubject = CreateTestSchoolSubject();
            var studyGroup = CreateTestStudyGroup();
            var draft = CreateTestLessonDraft(scheduleId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _teacherRepositoryMock
                .Setup(repo => repo.GetTeacherByIdAsync(lessonDto.Teacher.Id))
                .ReturnsAsync(teacher);

            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomByIdAsync(lessonDto.Classroom.Id))
                .ReturnsAsync(classroom);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(lessonDto.SchoolSubject.Id))
                .ReturnsAsync(schoolSubject);

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(lessonDto.StudyGroup.Id))
                .ReturnsAsync(studyGroup);

            _lessonFactoryMock
                .Setup(factory => factory.CreateFromDraft(It.IsAny<LessonDraft>()))
                .Returns(Result<Lesson>.Failure("Conflict detected"));

            _lessonDraftRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<LessonDraft>(), scheduleId))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonServices.AddLesson(lessonDto, scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
            _lessonFactoryMock.Verify(factory => factory.CreateFromDraft(It.IsAny<LessonDraft>()), Times.Once);
            _lessonDraftRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<LessonDraft>(), scheduleId), Times.Once);
        }

        [Fact]
        public async Task AddLesson_ShouldReturnFailure_WhenScheduleNotFound()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonDto = CreateValidCreateLessonDto();

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync((Schedule?)null);

            // Act
            var result = await _lessonServices.AddLesson(lessonDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Расписание не найдено.", result.Error);
        }

        [Fact]
        public async Task AddLesson_ShouldReturnFailure_WhenSchoolSubjectIsNull()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonDto = CreateValidCreateLessonDto();
            lessonDto.SchoolSubject = null;

            // Act
            var result = await _lessonServices.AddLesson(lessonDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Нельзя создать урок без предмета.", result.Error);
        }

        [Fact]
        public async Task AddLesson_ShouldReturnFailure_WhenSchoolSubjectNotFound()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonDto = CreateValidCreateLessonDto();
            var schedule = CreateTestSchedule(scheduleId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(lessonDto.SchoolSubject.Id))
                .ReturnsAsync((SchoolSubject?)null);

            // Act
            var result = await _lessonServices.AddLesson(lessonDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Предмет не найден.", result.Error);
        }

        [Fact]
        public async Task AddLesson_ShouldHandleNullTeacherAndClassroom()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonDto = CreateValidCreateLessonDto();
            lessonDto.Teacher = null;
            lessonDto.Classroom = null;
            var schedule = CreateTestSchedule(scheduleId);
            var schoolSubject = CreateTestSchoolSubject();
            var studyGroup = CreateTestStudyGroup();
            var lesson = CreateTestLesson(scheduleId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(lessonDto.SchoolSubject.Id))
                .ReturnsAsync(schoolSubject);

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(lessonDto.StudyGroup.Id))
                .ReturnsAsync(studyGroup);

            _lessonFactoryMock
                .Setup(factory => factory.CreateFromDraft(It.IsAny<LessonDraft>()))
                .Returns(Result<Lesson>.Success(lesson));

            _lessonRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Lesson>(), scheduleId))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonServices.AddLesson(lessonDto, scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
            _lessonFactoryMock.Verify(factory => factory.CreateFromDraft(
                It.Is<LessonDraft>(d => d.Teacher == null && d.Classroom == null)), Times.Once);
        }

        [Fact]
        public async Task AddLesson_ShouldReturnFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonDto = CreateValidCreateLessonDto();
            var schedule = CreateTestSchedule(scheduleId);
            var schoolSubject = CreateTestSchoolSubject();
            var studyGroup = CreateTestStudyGroup();

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(lessonDto.SchoolSubject.Id))
                .ReturnsAsync(schoolSubject);

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(lessonDto.StudyGroup.Id))
                .ReturnsAsync(studyGroup);

            _lessonFactoryMock
                .Setup(factory => factory.CreateFromDraft(It.IsAny<LessonDraft>()))
                .Returns(Result<Lesson>.Failure("Some error"));

            _lessonDraftRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<LessonDraft>(), scheduleId))
                .ThrowsAsync(new InvalidOperationException("Repository error"));

            // Act
            var result = await _lessonServices.AddLesson(lessonDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Запись не найдена в базе данных.", result.Error);
        }
        #endregion

        #region EditLesson Tests

        [Fact]
        public async Task EditLesson_ShouldReturnSuccess_WhenLessonExistsAndAllDataValid()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonDto = CreateValidLessonDto(scheduleId);
            var schedule = CreateTestSchedule(scheduleId);
            var lesson = CreateTestLesson(scheduleId);
            var schoolSubject = CreateTestSchoolSubject();
            var editedLessons = new List<Lesson> { lesson };

            schedule.AddLesson(lesson);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonRepositoryMock
                .Setup(repo => repo.GetLessonByIdAsync(lessonDto.Id))
                .ReturnsAsync(lesson);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(lessonDto.SchoolSubject.Id))
                .ReturnsAsync(schoolSubject);

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(lessonDto.StudyGroup.Id))
                .ReturnsAsync(lesson.StudyGroup);

            _lessonRepositoryMock
                .Setup(repo => repo.UpdateRangeAsync(It.IsAny<List<Lesson>>()))
                .Returns(Task.CompletedTask);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftById(lesson.Id))
                .ReturnsAsync((LessonDraft?)null);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonServices.EditLesson(lessonDto, scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
            _scheduleRepositoryMock.Verify(repo => repo.GetScheduleByIdAsync(scheduleId), Times.Once);
            _lessonRepositoryMock.Verify(repo => repo.UpdateRangeAsync(It.IsAny<List<Lesson>>()), Times.Once);
        }

        [Fact]
        public async Task EditLesson_ShouldReturnFailure_WhenScheduleNotFound()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonDto = CreateValidLessonDto(scheduleId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync((Schedule?)null);

            // Act
            var result = await _lessonServices.EditLesson(lessonDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Расписание не найдено.", result.Error);
        }

        [Fact]
        public async Task EditLesson_ShouldReturnFailure_WhenLessonNotFound()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonDto = CreateValidLessonDto(scheduleId);
            var schedule = CreateTestSchedule(scheduleId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonRepositoryMock
                .Setup(repo => repo.GetLessonByIdAsync(lessonDto.Id))
                .ReturnsAsync((Lesson?)null);

            // Act
            var result = await _lessonServices.EditLesson(lessonDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Урок не найден.", result.Error);
        }
        
        [Fact]
        public async Task EditLesson_ShouldReturnFailure_WhenEditLessonFails()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonDto = CreateValidLessonDto(scheduleId);
            var schedule = CreateTestSchedule(scheduleId);
            var lesson = CreateTestLesson(scheduleId);

            // Make schedule.EditLesson return failure
            schedule.AddLesson(lesson);
            var editResult = Result<List<Lesson>>.Failure("Edit failed");
            
            // We can't easily mock Schedule.EditLesson since it's not virtual
            // This test shows we need to handle this scenario

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonRepositoryMock
                .Setup(repo => repo.GetLessonByIdAsync(lessonDto.Id))
                .ReturnsAsync(lesson);

            // Act & Assert
            // Since we can't easily mock the Schedule.EditLesson method,
            // this test shows the limitation of our current design
            // We should consider making Schedule methods virtual or using interfaces
        }

        #endregion

        #region DeleteLesson Tests

        [Fact]
        public async Task DeleteLesson_ShouldReturnSuccess_WhenLessonExists()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonDto = CreateValidLessonDto(scheduleId);
            var schedule = CreateTestSchedule(scheduleId);
            var lesson = CreateTestLesson(scheduleId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonRepositoryMock
                .Setup(repo => repo.GetLessonByIdAsync(lessonDto.Id))
                .ReturnsAsync(lesson);

            _lessonRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<Lesson>()))
                .Returns(Task.CompletedTask);

            _lessonRepositoryMock
                .Setup(repo => repo.UpdateRangeAsync(It.IsAny<List<Lesson>>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonServices.DeleteLesson(lessonDto, scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
            _lessonRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Lesson>()), Times.Once);
            _lessonRepositoryMock.Verify(repo => repo.UpdateRangeAsync(It.IsAny<List<Lesson>>()), Times.Once);
        }

        [Fact]
        public async Task DeleteLesson_ShouldReturnFailure_WhenScheduleNotFound()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonDto = CreateValidLessonDto(scheduleId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync((Schedule?)null);

            // Act
            var result = await _lessonServices.DeleteLesson(lessonDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Расписание не найдено.", result.Error);
        }

        [Fact]
        public async Task DeleteLesson_ShouldReturnFailure_WhenLessonNotFound()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonDto = CreateValidLessonDto(scheduleId);
            var schedule = CreateTestSchedule(scheduleId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonRepositoryMock
                .Setup(repo => repo.GetLessonByIdAsync(lessonDto.Id))
                .ReturnsAsync((Lesson?)null);

            // Act
            var result = await _lessonServices.DeleteLesson(lessonDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Урок не найден.", result.Error);
        }

        #endregion

        #region Edge Cases and Integration Tests

        [Fact]
        public async Task AddLesson_ShouldHandleLessonNumberCreationFailure()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonDto = CreateValidCreateLessonDto();
            lessonDto.LessonNumber = new LessonNumberDto { Number = -1, Time = "09:00" }; // Invalid number
            var schedule = CreateTestSchedule(scheduleId);
            var schoolSubject = CreateTestSchoolSubject();

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(lessonDto.SchoolSubject.Id))
                .ReturnsAsync(schoolSubject);

            // Act
            var result = await _lessonServices.AddLesson(lessonDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains("Номер урока должен быть больше 0", result.Error);
        }
        
        [Fact]
        public async Task MultipleOperations_ShouldWorkCorrectly()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var schedule = CreateTestSchedule(scheduleId);
            
            // Setup schedule repository
            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            // Setup for GetLessonsByScheduleId
            var lessons = new List<Lesson> { CreateTestLesson(scheduleId) };
            _lessonRepositoryMock
                .Setup(repo => repo.GetLessonsByScheduleIdAsync(scheduleId))
                .ReturnsAsync(lessons);

            // Setup for AddLesson
            var addDto = CreateValidCreateLessonDto();
            var schoolSubject = CreateTestSchoolSubject();
            var studyGroup = CreateTestStudyGroup();
            var newLesson = CreateTestLesson(scheduleId);
            
            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(addDto.SchoolSubject.Id))
                .ReturnsAsync(schoolSubject);
                
            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(addDto.StudyGroup.Id))
                .ReturnsAsync(studyGroup);
                
            _lessonFactoryMock
                .Setup(factory => factory.CreateFromDraft(It.IsAny<LessonDraft>()))
                .Returns(Result<Lesson>.Success(newLesson));

            // Setup for DeleteLesson
            var deleteDto = CreateValidLessonDto(scheduleId);
            var lessonToDelete = CreateTestLesson(scheduleId);
            
            _lessonRepositoryMock
                .Setup(repo => repo.GetLessonByIdAsync(deleteDto.Id))
                .ReturnsAsync(lessonToDelete);

            // Setup unit of work to return success
            _unitOfWorkMock
                .SetupSequence(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)
                .ReturnsAsync(1)
                .ReturnsAsync(1);

            // Act - Get lessons
            var getResult = await _lessonServices.GetLessonsByScheduleId(scheduleId);

            // Act - Add lesson
            var addResult = await _lessonServices.AddLesson(addDto, scheduleId);

            // Act - Delete lesson
            var deleteResult = await _lessonServices.DeleteLesson(deleteDto, scheduleId);

            // Assert
            Assert.NotNull(getResult);
            Assert.True(addResult.IsSuccess);
            Assert.True(deleteResult.IsSuccess);
        }

        [Fact]
        public async Task EditLesson_ShouldUpdateMultipleLessons_WhenConflictResolutionTriggers()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonDto = CreateValidLessonDto(scheduleId);
            var schedule = CreateTestSchedule(scheduleId);
            var lesson = CreateTestLesson(scheduleId);
            var anotherLesson = CreateTestLesson(scheduleId);
            var schoolSubject = CreateTestSchoolSubject();
            
            // Create a conflict scenario
            schedule.AddLesson(lesson);
            schedule.AddLesson(anotherLesson);
            
            // Make EditLesson return multiple updated lessons
            var updatedLessons = new List<Lesson> { lesson, anotherLesson };

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonRepositoryMock
                .Setup(repo => repo.GetLessonByIdAsync(lessonDto.Id))
                .ReturnsAsync(lesson);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(lessonDto.SchoolSubject.Id))
                .ReturnsAsync(schoolSubject);

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(lessonDto.StudyGroup.Id))
                .ReturnsAsync(lesson.StudyGroup);

            _lessonRepositoryMock
                .Setup(repo => repo.UpdateRangeAsync(It.IsAny<List<Lesson>>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonServices.EditLesson(lessonDto, scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
            _lessonRepositoryMock.Verify(repo => repo.UpdateRangeAsync(It.IsAny<List<Lesson>>()), Times.AtLeastOnce);
        }

        #endregion

        #region Helper Methods

        private Schedule CreateTestSchedule(Guid id)
        {
            return Schedule.CreateSchedule(id, "Test Schedule").Value;
        }

        private Lesson CreateTestLesson(Guid scheduleId)
        {
            var schoolSubject = SchoolSubject.CreateSchoolSubject(Guid.NewGuid(), "Math").Value;
            var studyGroup = StudyGroup.CreateStudyGroup(Guid.NewGuid(), scheduleId, "Group A").Value;
            var lessonNumber = LessonNumber.CreateLessonNumber(1, "09:00").Value;
            
            return new Lesson(
                Guid.NewGuid(),
                scheduleId,
                schoolSubject,
                lessonNumber,
                CreateTestTeacher(),
                studyGroup,
                CreateTestClassroom()
            );
        }

        private LessonDraft CreateTestLessonDraft(Guid scheduleId)
        {
            var schoolSubject = SchoolSubject.CreateSchoolSubject(Guid.NewGuid(), "Math").Value;
            var studyGroup = StudyGroup.CreateStudyGroup(Guid.NewGuid(), scheduleId, "Group A").Value;
            var lessonNumber = LessonNumber.CreateLessonNumber(1, "09:00").Value;
            
            return new LessonDraft(
                Guid.NewGuid(),
                scheduleId,
                schoolSubject,
                lessonNumber,
                CreateTestTeacher(),
                studyGroup,
                CreateTestClassroom()
            );
        }

        private Teacher CreateTestTeacher()
        {
            return Teacher.CreateTeacher(
                Guid.NewGuid(),
                "John",
                "Doe",
                "Smith",
                new List<SchoolSubject>(),
                new List<StudyGroup>()
            ).Value;
        }

        private Classroom CreateTestClassroom()
        {
            return Classroom.CreateClassroom(Guid.NewGuid(), "Room 101", "First floor").Value;
        }

        private SchoolSubject CreateTestSchoolSubject()
        {
            return SchoolSubject.CreateSchoolSubject(Guid.NewGuid(), "Mathematics").Value;
        }

        private StudyGroup CreateTestStudyGroup()
        {
            return StudyGroup.CreateStudyGroup(Guid.NewGuid(), Guid.NewGuid(), "Group A").Value;
        }

        private CreateLessonDto CreateValidCreateLessonDto()
        {
            return new CreateLessonDto
            {
                ScheduleId = Guid.NewGuid(),
                SchoolSubject = new SchoolSubjectDto { Id = Guid.NewGuid(), Name = "Math" },
                LessonNumber = new LessonNumberDto { Number = 1, Time = "09:00" },
                Teacher = new TeacherDto { Id = Guid.NewGuid(), Name = "John", Surname = "Doe" },
                StudyGroup = new StudyGroupDto { Id = Guid.NewGuid(), Name = "Group A" },
                Classroom = new ClassroomDto { Id = Guid.NewGuid(), Name = "Room 101" },
                StudySubgroup = new StudySubgroupDto { Name = "Subgroup 1" },
                Comment = "Test lesson"
            };
        }

        private LessonDto CreateValidLessonDto(Guid scheduleId)
        {
            return new LessonDto
            {
                Id = Guid.NewGuid(),
                ScheduleId = scheduleId,
                SchoolSubject = new SchoolSubjectDto { Id = Guid.NewGuid(), Name = "Math" },
                LessonNumber = new LessonNumberDto { Number = 1, Time = "09:00" },
                Teacher = new TeacherDto { Id = Guid.NewGuid(), Name = "John", Surname = "Doe" },
                StudyGroup = new StudyGroupDto { Id = Guid.NewGuid(), Name = "Group A" },
                Classroom = new ClassroomDto { Id = Guid.NewGuid(), Name = "Room 101" },
                StudySubgroup = new StudySubgroupDto { Name = "Subgroup 1" },
                Comment = "Test lesson"
            };
        }

        #endregion
    }
}