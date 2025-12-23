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
    public class LessonDraftServicesTests
    {
        private readonly Mock<ILessonDraftRepository> _lessonDraftRepositoryMock;
        private readonly Mock<ILessonRepository> _lessonRepositoryMock;
        private readonly Mock<IScheduleRepository> _scheduleRepositoryMock;
        private readonly Mock<ISchoolSubjectRepository> _schoolSubjectRepositoryMock;
        private readonly Mock<IStudyGroupRepository> _studyGroupRepositoryMock;
        private readonly Mock<ILessonFactory> _lessonFactoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly LessonDraftServices _lessonDraftServices;

        public LessonDraftServicesTests()
        {
            _lessonDraftRepositoryMock = new Mock<ILessonDraftRepository>();
            _lessonRepositoryMock = new Mock<ILessonRepository>();
            _scheduleRepositoryMock = new Mock<IScheduleRepository>();
            _schoolSubjectRepositoryMock = new Mock<ISchoolSubjectRepository>();
            Mock<ITeacherRepository> teacherRepositoryMock = new Mock<ITeacherRepository>();
            _studyGroupRepositoryMock = new Mock<IStudyGroupRepository>();
            Mock<IClassroomRepository> classroomRepositoryMock = new Mock<IClassroomRepository>();
            _lessonFactoryMock = new Mock<ILessonFactory>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            Mock<ILogger<LessonDraftServices>> loggerMock = new Mock<ILogger<LessonDraftServices>>();

            _lessonDraftServices = new LessonDraftServices(
                _lessonDraftRepositoryMock.Object,
                _lessonRepositoryMock.Object,
                _scheduleRepositoryMock.Object,
                _schoolSubjectRepositoryMock.Object,
                teacherRepositoryMock.Object,
                _studyGroupRepositoryMock.Object,
                classroomRepositoryMock.Object,
                _lessonFactoryMock.Object,
                _unitOfWorkMock.Object,
                loggerMock.Object
            );
        }

        #region GetLessonDraftsByScheduleId Tests

        [Fact]
        public async Task GetLessonDraftsByScheduleId_ShouldReturnDrafts_WhenScheduleExists()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var drafts = new List<LessonDraft>
            {
                CreateTestLessonDraft(scheduleId),
                CreateTestLessonDraft(scheduleId)
            };

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftsByScheduleId(scheduleId))
                .ReturnsAsync(drafts);

            // Act
            var result = await _lessonDraftServices.GetLessonDraftsByScheduleId(scheduleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _lessonDraftRepositoryMock.Verify(repo => repo.GetLessonDraftsByScheduleId(scheduleId), Times.Once);
        }

        [Fact]
        public async Task GetLessonDraftsByScheduleId_ShouldReturnEmptyList_WhenNoDrafts()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var emptyList = new List<LessonDraft>();

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftsByScheduleId(scheduleId))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _lessonDraftServices.GetLessonDraftsByScheduleId(scheduleId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetLessonDraftsByScheduleId_ShouldHandleRepositoryException()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftsByScheduleId(scheduleId))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _lessonDraftServices.GetLessonDraftsByScheduleId(scheduleId));
        }

        [Fact]
        public async Task GetLessonDraftsByScheduleId_ShouldReturnOnlyDraftsForSpecifiedSchedule()
        {
            // Arrange
            var scheduleId1 = Guid.NewGuid();
            Guid.NewGuid();
            
            var draftsForSchedule1 = new List<LessonDraft>
            {
                CreateTestLessonDraft(scheduleId1),
                CreateTestLessonDraft(scheduleId1)
            };

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftsByScheduleId(scheduleId1))
                .ReturnsAsync(draftsForSchedule1);

            // Act
            var result = await _lessonDraftServices.GetLessonDraftsByScheduleId(scheduleId1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, dto => Assert.Equal(scheduleId1, dto.ScheduleId));
        }

        #endregion

        #region GetLessonDraftById Tests

        [Fact]
        public async Task GetLessonDraftById_ShouldReturnSuccess_WhenDraftExists()
        {
            // Arrange
            var draftId = Guid.NewGuid();
            var scheduleId = Guid.NewGuid();
            var draft = CreateTestLessonDraft(scheduleId, draftId);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftById(draftId))
                .ReturnsAsync(draft);

            // Act
            var result = await _lessonDraftServices.GetLessonDraftById(draftId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(draftId, result.Value.Id);
            Assert.Equal(scheduleId, result.Value.ScheduleId);
            _lessonDraftRepositoryMock.Verify(repo => repo.GetLessonDraftById(draftId), Times.Once);
        }

        [Fact]
        public async Task GetLessonDraftById_ShouldReturnFailure_WhenDraftNotFound()
        {
            // Arrange
            var draftId = Guid.NewGuid();

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftById(draftId))
                .ReturnsAsync((LessonDraft?)null);

            // Act
            var result = await _lessonDraftServices.GetLessonDraftById(draftId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Урок не найден.", result.Error);
            _lessonDraftRepositoryMock.Verify(repo => repo.GetLessonDraftById(draftId), Times.Once);
        }

        [Fact]
        public async Task GetLessonDraftById_ShouldHandleRepositoryException()
        {
            // Arrange
            var draftId = Guid.NewGuid();

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftById(draftId))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _lessonDraftServices.GetLessonDraftById(draftId));
        }

        [Fact]
        public async Task GetLessonDraftById_ShouldHandleDefaultGuid()
        {
            // Arrange
            var draftId = Guid.Empty;

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftById(draftId))
                .ReturnsAsync((LessonDraft?)null);

            // Act
            var result = await _lessonDraftServices.GetLessonDraftById(draftId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Урок не найден.", result.Error);
        }

        #endregion

        #region EditDraftLesson Tests

        [Fact]
        public async Task EditDraftLesson_ShouldReturnSuccess_WhenAllRequiredFieldsPresentAndFactorySucceeds()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var draftId = Guid.NewGuid();
            var lessonDraftDto = CreateValidLessonDraftDto(scheduleId, draftId);
            var schedule = CreateTestSchedule(scheduleId);
            var lessonDraft = CreateTestLessonDraft(scheduleId, draftId);
            var schoolSubject = CreateTestSchoolSubject();
            var lesson = CreateTestLesson(scheduleId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftById(draftId))
                .ReturnsAsync(lessonDraft);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(lessonDraftDto.SchoolSubject.Id))
                .ReturnsAsync(schoolSubject);

            _lessonFactoryMock
                .Setup(factory => factory.CreateFromDraft(It.IsAny<LessonDraft>()))
                .Returns(Result<Lesson>.Success(lesson));

            _lessonRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Lesson>(), scheduleId))
                .Returns(Task.CompletedTask);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<LessonDraft>()))
                .Returns(Task.CompletedTask);

            _lessonRepositoryMock
                .Setup(repo => repo.UpdateRangeAsync(It.IsAny<List<Lesson>>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonDraftServices.EditDraftLesson(lessonDraftDto, scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
            _lessonFactoryMock.Verify(factory => factory.CreateFromDraft(It.IsAny<LessonDraft>()), Times.Once);
            _lessonRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Lesson>(), scheduleId), Times.Once);
            _lessonDraftRepositoryMock.Verify(repo => repo.Delete(It.IsAny<LessonDraft>()), Times.Once);
        }

        [Fact]
        public async Task EditDraftLesson_ShouldReturnDowngradedResult_WhenRequiredFieldsMissing()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var draftId = Guid.NewGuid();
            var lessonDraftDto = CreateValidLessonDraftDto(scheduleId, draftId);
            lessonDraftDto.SchoolSubject = null;
            var schedule = CreateTestSchedule(scheduleId);
            var lessonDraft = CreateTestLessonDraft(scheduleId, draftId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftById(draftId))
                .ReturnsAsync(lessonDraft);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.Update(It.IsAny<LessonDraft>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonDraftServices.EditDraftLesson(lessonDraftDto, scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
            _lessonFactoryMock.Verify(factory => factory.CreateFromDraft(It.IsAny<LessonDraft>()), Times.Never);
            _lessonDraftRepositoryMock.Verify(repo => repo.Update(It.IsAny<LessonDraft>()), Times.Once);
        }

        [Fact]
        public async Task EditDraftLesson_ShouldReturnFailure_WhenScheduleNotFound()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var draftId = Guid.NewGuid();
            var lessonDraftDto = CreateValidLessonDraftDto(scheduleId, draftId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync((Schedule?)null);

            // Act
            var result = await _lessonDraftServices.EditDraftLesson(lessonDraftDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Расписание не найдено.", result.Error);
            _lessonDraftRepositoryMock.Verify(repo => repo.GetLessonDraftById(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task EditDraftLesson_ShouldReturnFailure_WhenLessonDraftNotFound()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var draftId = Guid.NewGuid();
            var lessonDraftDto = CreateValidLessonDraftDto(scheduleId, draftId);
            var schedule = CreateTestSchedule(scheduleId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftById(draftId))
                .ReturnsAsync((LessonDraft?)null);

            // Act
            var result = await _lessonDraftServices.EditDraftLesson(lessonDraftDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Урок не найден.", result.Error);
            _lessonFactoryMock.Verify(factory => factory.CreateFromDraft(It.IsAny<LessonDraft>()), Times.Never);
        }

        [Fact]
        public async Task EditDraftLesson_ShouldReturnFailure_WhenSchoolSubjectIsNull()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var draftId = Guid.NewGuid();
            var lessonDraftDto = CreateValidLessonDraftDto(scheduleId, draftId);
            lessonDraftDto.SchoolSubject = null;
            var schedule = CreateTestSchedule(scheduleId);
            var lessonDraft = CreateTestLessonDraft(scheduleId, draftId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftById(draftId))
                .ReturnsAsync(lessonDraft);

            // Act
            var result = await _lessonDraftServices.EditDraftLesson(lessonDraftDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Нельзя убрать предмет у урока.", result.Error);
            _lessonFactoryMock.Verify(factory => factory.CreateFromDraft(It.IsAny<LessonDraft>()), Times.Never);
        }

        [Fact]
        public async Task EditDraftLesson_ShouldReturnFailure_WhenSchoolSubjectNotFound()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var draftId = Guid.NewGuid();
            var lessonDraftDto = CreateValidLessonDraftDto(scheduleId, draftId);
            var schedule = CreateTestSchedule(scheduleId);
            var lessonDraft = CreateTestLessonDraft(scheduleId, draftId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftById(draftId))
                .ReturnsAsync(lessonDraft);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(lessonDraftDto.SchoolSubject.Id))
                .ReturnsAsync((SchoolSubject?)null);

            // Act
            var result = await _lessonDraftServices.EditDraftLesson(lessonDraftDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Предмет не найден.", result.Error);
            _lessonFactoryMock.Verify(factory => factory.CreateFromDraft(It.IsAny<LessonDraft>()), Times.Never);
        }

        [Fact]
        public async Task EditDraftLesson_ShouldReturnFailure_WhenLessonNumberCreationFails()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var draftId = Guid.NewGuid();
            var lessonDraftDto = CreateValidLessonDraftDto(scheduleId, draftId);
            lessonDraftDto.LessonNumber = new LessonNumberDto { Number = -1, Time = "09:00" }; 
            var schedule = CreateTestSchedule(scheduleId);
            var lessonDraft = CreateTestLessonDraft(scheduleId, draftId);
            var schoolSubject = CreateTestSchoolSubject();

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftById(draftId))
                .ReturnsAsync(lessonDraft);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(lessonDraftDto.SchoolSubject.Id))
                .ReturnsAsync(schoolSubject);

            // Act
            var result = await _lessonDraftServices.EditDraftLesson(lessonDraftDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains("Номер урока должен быть больше 0", result.Error);
            _lessonFactoryMock.Verify(factory => factory.CreateFromDraft(It.IsAny<LessonDraft>()), Times.Never);
        }

        [Fact]
        public async Task EditDraftLesson_ShouldReturnFailure_WhenStudySubgroupCreationFails()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var draftId = Guid.NewGuid();
            var lessonDraftDto = CreateValidLessonDraftDto(scheduleId, draftId);
            lessonDraftDto.StudySubgroup = new StudySubgroupDto { Name = "" }; 
            var schedule = CreateTestSchedule(scheduleId);
            var lessonDraft = CreateTestLessonDraft(scheduleId, draftId);
            var schoolSubject = CreateTestSchoolSubject();
            var studyGroup = CreateTestStudyGroup(scheduleId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftById(draftId))
                .ReturnsAsync(lessonDraft);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(lessonDraftDto.SchoolSubject.Id))
                .ReturnsAsync(schoolSubject);

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(lessonDraftDto.StudyGroup.Id))
                .ReturnsAsync(studyGroup);

            // Act
            var result = await _lessonDraftServices.EditDraftLesson(lessonDraftDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains("Название учебной подгруппы не может быть пустым", result.Error);
            _lessonFactoryMock.Verify(factory => factory.CreateFromDraft(It.IsAny<LessonDraft>()), Times.Never);
        }

        [Fact]
        public async Task EditDraftLesson_ShouldReturnFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var draftId = Guid.NewGuid();
            var lessonDraftDto = CreateValidLessonDraftDto(scheduleId, draftId);
            var schedule = CreateTestSchedule(scheduleId);
            var lessonDraft = CreateTestLessonDraft(scheduleId, draftId);
            var schoolSubject = CreateTestSchoolSubject();
            var studyGroup = CreateTestStudyGroup(scheduleId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftById(draftId))
                .ReturnsAsync(lessonDraft);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(lessonDraftDto.SchoolSubject.Id))
                .ReturnsAsync(schoolSubject);

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(lessonDraftDto.StudyGroup.Id))
                .ReturnsAsync(studyGroup);

            _lessonFactoryMock
                .Setup(factory => factory.CreateFromDraft(It.IsAny<LessonDraft>()))
                .Returns(Result<Lesson>.Success(CreateTestLesson(scheduleId)));

            _lessonRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Lesson>(), scheduleId))
                .ThrowsAsync(new InvalidOperationException("Repository error"));

            // Act
            var result = await _lessonDraftServices.EditDraftLesson(lessonDraftDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Запись не найдена в базе данных.", result.Error);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region DeleteLessonDraft Tests

        [Fact]
        public async Task DeleteLessonDraft_ShouldReturnSuccess_WhenDraftExists()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var draftId = Guid.NewGuid();
            var lessonDraftDto = new LessonDraftDto 
            { 
                Id = draftId, 
                ScheduleId = scheduleId 
            };
            var lessonDraft = CreateTestLessonDraft(scheduleId, draftId);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftById(draftId))
                .ReturnsAsync(lessonDraft);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<LessonDraft>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonDraftServices.DeleteLessonDraft(lessonDraftDto, scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
            _lessonDraftRepositoryMock.Verify(repo => repo.GetLessonDraftById(draftId), Times.Once);
            _lessonDraftRepositoryMock.Verify(repo => repo.Delete(
                It.Is<LessonDraft>(d => d.Id == draftId)), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteLessonDraft_ShouldReturnFailure_WhenDraftNotFound()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var draftId = Guid.NewGuid();
            var lessonDraftDto = new LessonDraftDto 
            { 
                Id = draftId, 
                ScheduleId = scheduleId 
            };

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftById(draftId))
                .ReturnsAsync((LessonDraft?)null);

            // Act
            var result = await _lessonDraftServices.DeleteLessonDraft(lessonDraftDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Урок не найден.", result.Error);
            _lessonDraftRepositoryMock.Verify(repo => repo.Delete(It.IsAny<LessonDraft>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteLessonDraft_ShouldNotCheckScheduleId_WhenDeleting()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var differentScheduleId = Guid.NewGuid(); 
            var draftId = Guid.NewGuid();
            var lessonDraftDto = new LessonDraftDto 
            { 
                Id = draftId, 
                ScheduleId = differentScheduleId 
            };
            var lessonDraft = CreateTestLessonDraft(scheduleId, draftId); 

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftById(draftId))
                .ReturnsAsync(lessonDraft);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<LessonDraft>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonDraftServices.DeleteLessonDraft(lessonDraftDto, scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
        }

        #endregion

        #region ClearDraftsByScheduleId Tests

        [Fact]
        public async Task ClearDraftsByScheduleId_ShouldReturnSuccess_WhenDraftsExist()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var drafts = new List<LessonDraft>
            {
                CreateTestLessonDraft(scheduleId),
                CreateTestLessonDraft(scheduleId)
            };

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftsByScheduleId(scheduleId))
                .ReturnsAsync(drafts);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<LessonDraft>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonDraftServices.ClearDraftsByScheduleId(scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
            _lessonDraftRepositoryMock.Verify(repo => repo.GetLessonDraftsByScheduleId(scheduleId), Times.Once);
            _lessonDraftRepositoryMock.Verify(repo => repo.Delete(It.IsAny<LessonDraft>()), Times.Exactly(2));
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ClearDraftsByScheduleId_ShouldReturnSuccess_WhenNoDrafts()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var emptyList = new List<LessonDraft>();

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftsByScheduleId(scheduleId))
                .ReturnsAsync(emptyList);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonDraftServices.ClearDraftsByScheduleId(scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
            _lessonDraftRepositoryMock.Verify(repo => repo.GetLessonDraftsByScheduleId(scheduleId), Times.Once);
            _lessonDraftRepositoryMock.Verify(repo => repo.Delete(It.IsAny<LessonDraft>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ClearDraftsByScheduleId_ShouldHandleMultipleDraftsDeletion()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var drafts = new List<LessonDraft>
            {
                CreateTestLessonDraft(scheduleId),
                CreateTestLessonDraft(scheduleId),
                CreateTestLessonDraft(scheduleId)
            };

            var deleteCount = 0;
            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftsByScheduleId(scheduleId))
                .ReturnsAsync(drafts);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<LessonDraft>()))
                .Callback(() => deleteCount++)
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonDraftServices.ClearDraftsByScheduleId(scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(3, deleteCount);
        }

        [Fact]
        public async Task ClearDraftsByScheduleId_ShouldDeleteOnlyDraftsForSpecifiedSchedule()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var otherScheduleId = Guid.NewGuid();
            
            var draftsForSchedule = new List<LessonDraft>
            {
                CreateTestLessonDraft(scheduleId),
                CreateTestLessonDraft(scheduleId)
            };

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftsByScheduleId(scheduleId))
                .ReturnsAsync(draftsForSchedule);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<LessonDraft>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonDraftServices.ClearDraftsByScheduleId(scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
            _lessonDraftRepositoryMock.Verify(repo => repo.GetLessonDraftsByScheduleId(scheduleId), Times.Once);
            _lessonDraftRepositoryMock.Verify(repo => repo.GetLessonDraftsByScheduleId(otherScheduleId), Times.Never);
        }

        #endregion

        #region Integration and Edge Case Tests

        [Fact]
        public async Task FullWorkflow_WithDraftCreationAndPromotion_ShouldWorkCorrectly()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            
            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftsByScheduleId(scheduleId))
                .ReturnsAsync([]);
            
            var draftId = Guid.NewGuid();
            var incompleteDraftDto = CreateValidLessonDraftDto(scheduleId, draftId);
            incompleteDraftDto.LessonNumber = null;
            var schedule = CreateTestSchedule(scheduleId);
            var lessonDraft = CreateTestLessonDraft(scheduleId, draftId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftById(draftId))
                .ReturnsAsync(lessonDraft);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.Update(It.IsAny<LessonDraft>()))
                .Returns(Task.CompletedTask);
            
            var completeDraftDto = CreateValidLessonDraftDto(scheduleId, draftId);
            var schoolSubject = CreateTestSchoolSubject();
            var studyGroup = CreateTestStudyGroup(scheduleId);
            var lesson = CreateTestLesson(scheduleId);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(completeDraftDto.SchoolSubject.Id))
                .ReturnsAsync(schoolSubject);

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(completeDraftDto.StudyGroup.Id))
                .ReturnsAsync(studyGroup);

            _lessonFactoryMock
                .Setup(factory => factory.CreateFromDraft(It.IsAny<LessonDraft>()))
                .Returns(Result<Lesson>.Success(lesson));

            _lessonRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Lesson>(), scheduleId))
                .Returns(Task.CompletedTask);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<LessonDraft>()))
                .Returns(Task.CompletedTask);
            
            var remainingDrafts = new List<LessonDraft> { CreateTestLessonDraft(scheduleId) };
            _lessonDraftRepositoryMock
                .SetupSequence(repo => repo.GetLessonDraftsByScheduleId(scheduleId))
                .ReturnsAsync([lessonDraft])
                .ReturnsAsync(remainingDrafts); 

            // Setup UnitOfWork to always succeed
            _unitOfWorkMock
                .SetupSequence(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)  // For updating draft
                .ReturnsAsync(1)  // For promoting to lesson
                .ReturnsAsync(1); // For clearing drafts

            // Act - Get initial drafts
            var initialDrafts = await _lessonDraftServices.GetLessonDraftsByScheduleId(scheduleId);

            // Act - Edit draft (incomplete -> stays as draft)
            var editResult1 = await _lessonDraftServices.EditDraftLesson(incompleteDraftDto, scheduleId);

            // Act - Edit draft (complete -> becomes lesson)
            var editResult2 = await _lessonDraftServices.EditDraftLesson(completeDraftDto, scheduleId);

            // Act - Clear remaining drafts
            var clearResult = await _lessonDraftServices.ClearDraftsByScheduleId(scheduleId);

            // Assert
            Assert.Empty(initialDrafts);
            Assert.True(editResult1.IsSuccess);
            Assert.True(editResult2.IsSuccess);
            Assert.True(clearResult.IsSuccess);
        }

        [Fact]
        public async Task EditDraftLesson_ShouldHandleStudyGroupNull_WhenStudySubgroupNotNull()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var draftId = Guid.NewGuid();
            var lessonDraftDto = CreateValidLessonDraftDto(scheduleId, draftId);
            lessonDraftDto.StudyGroup = null;
            lessonDraftDto.StudySubgroup = new StudySubgroupDto { Name = "Subgroup" };
            var schedule = CreateTestSchedule(scheduleId);
            var lessonDraft = CreateTestLessonDraft(scheduleId, draftId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftById(draftId))
                .ReturnsAsync(lessonDraft);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.Update(It.IsAny<LessonDraft>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonDraftServices.EditDraftLesson(lessonDraftDto, scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EditDraftLesson_ShouldUpdateAllFields_WhenStayingAsDraft()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var draftId = Guid.NewGuid();
            var lessonDraftDto = CreateValidLessonDraftDto(scheduleId, draftId);
            lessonDraftDto.LessonNumber = null; 
            var schedule = CreateTestSchedule(scheduleId);
            var lessonDraft = CreateTestLessonDraft(scheduleId, draftId);
            var schoolSubject = CreateTestSchoolSubject();

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftById(draftId))
                .ReturnsAsync(lessonDraft);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(lessonDraftDto.SchoolSubject.Id))
                .ReturnsAsync(schoolSubject);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.Update(It.IsAny<LessonDraft>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonDraftServices.EditDraftLesson(lessonDraftDto, scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
            _lessonDraftRepositoryMock.Verify(repo => repo.Update(It.IsAny<LessonDraft>()), Times.Once);
        }

        [Fact]
        public async Task DeleteLessonDraft_ShouldWorkWithInvalidDto_AsLongAsDraftExists()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var draftId = Guid.NewGuid();
            var lessonDraftDto = new LessonDraftDto 
            { 
                Id = draftId, 
                ScheduleId = Guid.Empty, 
                SchoolSubject = null,
                LessonNumber = null
            };
            var lessonDraft = CreateTestLessonDraft(scheduleId, draftId);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftById(draftId))
                .ReturnsAsync(lessonDraft);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<LessonDraft>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonDraftServices.DeleteLessonDraft(lessonDraftDto, scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Service_ShouldUseBaseServiceFunctionality()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var draftId = Guid.NewGuid();
            var lessonDraftDto = CreateValidLessonDraftDto(scheduleId, draftId);
            var schedule = CreateTestSchedule(scheduleId);
            var lessonDraft = CreateTestLessonDraft(scheduleId, draftId);
            var schoolSubject = CreateTestSchoolSubject();
            var studyGroup = CreateTestStudyGroup(scheduleId);
            var lesson = CreateTestLesson(scheduleId);
            
            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonDraftRepositoryMock
                .Setup(repo => repo.GetLessonDraftById(draftId))
                .ReturnsAsync(lessonDraft);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(lessonDraftDto.SchoolSubject.Id))
                .ReturnsAsync(schoolSubject);

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(lessonDraftDto.StudyGroup.Id))
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
            var result = await _lessonDraftServices.EditDraftLesson(lessonDraftDto, scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
        }

        #endregion

        #region Helper Methods

        private static Schedule CreateTestSchedule(Guid id)
        {
            return Schedule.CreateSchedule(id, "Test Schedule").Value;
        }

        private static LessonDraft CreateTestLessonDraft(Guid scheduleId, Guid? id = null)
        {
            var schoolSubject = SchoolSubject.CreateSchoolSubject(Guid.NewGuid(), "Math").Value;
            var studyGroup = StudyGroup.CreateStudyGroup(Guid.NewGuid(), scheduleId, "Group A").Value;
            var lessonNumber = LessonNumber.CreateLessonNumber(1, "09:00").Value;
            var teacher = Teacher.CreateTeacher(
                Guid.NewGuid(),
                "John",
                "Doe",
                "Smith",
                [],
                []
            ).Value;
            var classroom = Classroom.CreateClassroom(Guid.NewGuid(), "Room 101", null).Value;
            
            return new LessonDraft(
                id ?? Guid.NewGuid(),
                scheduleId,
                schoolSubject,
                lessonNumber,
                teacher,
                studyGroup,
                classroom
            );
        }

        private static Lesson CreateTestLesson(Guid scheduleId, Guid? id = null)
        {
            var schoolSubject = SchoolSubject.CreateSchoolSubject(Guid.NewGuid(), "Math").Value;
            var studyGroup = StudyGroup.CreateStudyGroup(Guid.NewGuid(), scheduleId, "Group A").Value;
            var lessonNumber = LessonNumber.CreateLessonNumber(1, "09:00").Value;
            var teacher = Teacher.CreateTeacher(
                Guid.NewGuid(),
                "John",
                "Doe",
                "Smith",
                [],
                []
            ).Value;
            var classroom = Classroom.CreateClassroom(Guid.NewGuid(), "Room 101", null).Value;
            
            return new Lesson(
                id ?? Guid.NewGuid(),
                scheduleId,
                schoolSubject,
                lessonNumber,
                teacher,
                studyGroup,
                classroom
            );
        }

        private static SchoolSubject CreateTestSchoolSubject()
        {
            return SchoolSubject.CreateSchoolSubject(Guid.NewGuid(), "Mathematics").Value;
        }

        private static StudyGroup CreateTestStudyGroup(Guid scheduleId)
        {
            return StudyGroup.CreateStudyGroup(Guid.NewGuid(), scheduleId, "Group A").Value;
        }

        private static LessonDraftDto CreateValidLessonDraftDto(Guid scheduleId, Guid draftId)
        {
            return new LessonDraftDto
            {
                Id = draftId,
                ScheduleId = scheduleId,
                SchoolSubject = new SchoolSubjectDto { Id = Guid.NewGuid(), Name = "Math" },
                LessonNumber = new LessonNumberDto { Number = 1, Time = "09:00" },
                Teacher = new TeacherDto { Id = Guid.NewGuid(), Name = "John", Surname = "Doe" },
                StudyGroup = new StudyGroupDto { Id = Guid.NewGuid(), Name = "Group A", ScheduleId = scheduleId },
                Classroom = new ClassroomDto { Id = Guid.NewGuid(), Name = "Room 101" },
                StudySubgroup = new StudySubgroupDto { Name = "Subgroup 1" },
                Comment = "Test draft"
            };
        }

        #endregion
    }
}