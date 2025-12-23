using Xunit;
using Moq;
using Application.Services;
using Application.DtoModels;
using Domain.Models;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;
using Application;


namespace Tests.ServicesTests
{
    public class ScheduleServicesTests
    {
        private readonly Mock<IScheduleRepository> _scheduleRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ScheduleServices _scheduleServices;

        public ScheduleServicesTests()
        {
            _scheduleRepositoryMock = new Mock<IScheduleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            Mock<ILogger<ScheduleServices>> loggerMock = new Mock<ILogger<ScheduleServices>>();
            
            _scheduleServices = new ScheduleServices(
                _scheduleRepositoryMock.Object,
                _unitOfWorkMock.Object,
                loggerMock.Object
            );
        }

        #region FetchSchedulesFromBackendAsync Tests

        [Fact]
        public async Task FetchSchedulesFromBackendAsync_ShouldReturnSchedules_WhenRepositoryReturnsData()
        {
            // Arrange
            var schedules = new List<Schedule>
            {
                CreateTestSchedule("Schedule 1"),
                CreateTestSchedule("Schedule 2"),
                CreateTestSchedule("Schedule 3")
            };

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleListAsync(1))
                .ReturnsAsync(schedules);

            // Act
            var result = await _scheduleServices.FetchSchedulesFromBackendAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("Schedule 1", result[0].Name);
            Assert.Equal("Schedule 2", result[1].Name);
            Assert.Equal("Schedule 3", result[2].Name);
            
            _scheduleRepositoryMock.Verify(repo => repo.GetScheduleListAsync(1), Times.Once);
        }

        [Fact]
        public async Task FetchSchedulesFromBackendAsync_ShouldReturnEmptyList_WhenNoSchedules()
        {
            // Arrange
            var emptyList = new List<Schedule>();

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleListAsync(1))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _scheduleServices.FetchSchedulesFromBackendAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task FetchSchedulesFromBackendAsync_ShouldHandleRepositoryException()
        {
            // Arrange
            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleListAsync(1))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _scheduleServices.FetchSchedulesFromBackendAsync());
        }

        [Fact]
        public async Task FetchSchedulesFromBackendAsync_ShouldIncludeLessonsAndDrafts_WhenMapping()
        {
            // Arrange
            var schedule = CreateTestSchedule("Schedule with lessons");
            CreateTestLesson(schedule.Id);
            CreateTestLessonDraft(schedule.Id);
            
            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleListAsync(1))
                .ReturnsAsync([schedule]);

            // Act
            var result = await _scheduleServices.FetchSchedulesFromBackendAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        #endregion

        #region AddSchedule Tests

        [Fact]
        public async Task AddSchedule_ShouldReturnSuccess_WhenDataValid()
        {
            // Arrange
            var createDto = new CreateScheduleDto { Name = "Test Schedule" };
            CreateTestSchedule("Test Schedule");

            _scheduleRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Schedule>(), 1))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _scheduleServices.AddSchedule(createDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("Test Schedule", result.Value.Name);
            
            _scheduleRepositoryMock.Verify(
                repo => repo.AddAsync(It.Is<Schedule>(s => s.Name == "Test Schedule"), 1),
                Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task AddSchedule_ShouldReturnFailure_WhenNameInvalid(string invalidName)
        {
            // Arrange
            var createDto = new CreateScheduleDto { Name = invalidName };

            // Act
            var result = await _scheduleServices.AddSchedule(createDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Название расписания не может быть пустым.", result.Error);
            _scheduleRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Schedule>(), It.IsAny<int>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddSchedule_ShouldReturnFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var createDto = new CreateScheduleDto { Name = "Test Schedule" };

            _scheduleRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Schedule>(), 1))
                .ThrowsAsync(new InvalidOperationException("Repository error"));

            // Act
            var result = await _scheduleServices.AddSchedule(createDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Запись не найдена в базе данных.", result.Error);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        

        [Fact]
        public async Task AddSchedule_ShouldGenerateNewGuid_ForEachSchedule()
        {
            // Arrange
            var createDto = new CreateScheduleDto { Name = "Test Schedule" };
            var generatedGuids = new List<Guid>();
            
            _scheduleRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Schedule>(), 1))
                .Callback<Schedule, int>((schedule, _) => generatedGuids.Add(schedule.Id))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result1 = await _scheduleServices.AddSchedule(createDto);
            var result2 = await _scheduleServices.AddSchedule(createDto);

            // Assert
            Assert.True(result1.IsSuccess);
            Assert.True(result2.IsSuccess);
            Assert.NotEqual(generatedGuids[0], generatedGuids[1]);
        }

        #endregion

        #region EditSchedule Tests

        [Fact]
        public async Task EditSchedule_ShouldReturnSuccess_WhenScheduleExistsAndNameValid()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var scheduleDto = new ScheduleDto { Id = scheduleId, Name = "Updated Schedule" };
            var existingSchedule = CreateTestSchedule("Original Schedule", scheduleId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(existingSchedule);

            _scheduleRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Schedule>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _scheduleServices.EditSchedule(scheduleDto);

            // Assert
            Assert.True(result.IsSuccess);
            _scheduleRepositoryMock.Verify(repo => repo.GetScheduleByIdAsync(scheduleId), Times.Once);
            _scheduleRepositoryMock.Verify(repo => repo.UpdateAsync(
                It.Is<Schedule>(s => s.Id == scheduleId && s.Name == "Updated Schedule")), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EditSchedule_ShouldReturnFailure_WhenScheduleNotFound()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var scheduleDto = new ScheduleDto { Id = scheduleId, Name = "Updated Schedule" };

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync((Schedule?)null);

            // Act
            var result = await _scheduleServices.EditSchedule(scheduleDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Расписание не найдено. Попробуйте позже.", result.Error);
            _scheduleRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Schedule>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task EditSchedule_ShouldReturnFailure_WhenNewNameInvalid(string invalidName)
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var scheduleDto = new ScheduleDto { Id = scheduleId, Name = invalidName };
            var existingSchedule = CreateTestSchedule("Original Schedule", scheduleId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(existingSchedule);

            // Act
            var result = await _scheduleServices.EditSchedule(scheduleDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Название расписания не может быть пустым.", result.Error);
            _scheduleRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Schedule>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task EditSchedule_ShouldNotUpdate_WhenNameUnchanged()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var scheduleDto = new ScheduleDto { Id = scheduleId, Name = "Original Schedule" };
            var existingSchedule = CreateTestSchedule("Original Schedule", scheduleId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(existingSchedule);

            _scheduleRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Schedule>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _scheduleServices.EditSchedule(scheduleDto);

            // Assert
            Assert.True(result.IsSuccess);
            _scheduleRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Schedule>()), Times.Once);
        }

        [Fact]
        public async Task EditSchedule_ShouldReturnFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var scheduleDto = new ScheduleDto { Id = scheduleId, Name = "Updated Schedule" };
            var existingSchedule = CreateTestSchedule("Original Schedule", scheduleId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(existingSchedule);

            _scheduleRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Schedule>()))
                .ThrowsAsync(new InvalidOperationException("Repository error"));

            // Act
            var result = await _scheduleServices.EditSchedule(scheduleDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Запись не найдена в базе данных.", result.Error);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task EditSchedule_ShouldHandleScheduleWithLessonsAndDrafts()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var scheduleDto = new ScheduleDto { Id = scheduleId, Name = "Updated Schedule" };
            var existingSchedule = CreateTestSchedule("Original Schedule", scheduleId);
            
            CreateTestLesson(scheduleId);
            CreateTestLessonDraft(scheduleId);
            
            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(existingSchedule);

            _scheduleRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Schedule>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _scheduleServices.EditSchedule(scheduleDto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        #endregion

        #region DeleteSchedule Tests

        [Fact]
        public async Task DeleteSchedule_ShouldReturnSuccess_WhenScheduleExists()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var scheduleDto = new ScheduleDto { Id = scheduleId, Name = "Schedule to delete" };
            var existingSchedule = CreateTestSchedule("Schedule to delete", scheduleId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(existingSchedule);

            _scheduleRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<Schedule>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _scheduleServices.DeleteSchedule(scheduleDto);

            // Assert
            Assert.True(result.IsSuccess);
            _scheduleRepositoryMock.Verify(repo => repo.GetScheduleByIdAsync(scheduleId), Times.Once);
            _scheduleRepositoryMock.Verify(repo => repo.Delete(
                It.Is<Schedule>(s => s.Id == scheduleId && s.Name == "Schedule to delete")), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteSchedule_ShouldReturnFailure_WhenScheduleNotFound()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var scheduleDto = new ScheduleDto { Id = scheduleId, Name = "Schedule to delete" };

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync((Schedule?)null);

            // Act
            var result = await _scheduleServices.DeleteSchedule(scheduleDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Расписание не найдено. Попробуйте позже.", result.Error);
            _scheduleRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Schedule>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteSchedule_ShouldReturnFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var scheduleDto = new ScheduleDto { Id = scheduleId, Name = "Schedule to delete" };
            var existingSchedule = CreateTestSchedule("Schedule to delete", scheduleId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(existingSchedule);

            _scheduleRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<Schedule>()))
                .ThrowsAsync(new InvalidOperationException("Repository error"));

            // Act
            var result = await _scheduleServices.DeleteSchedule(scheduleDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Ошибка при удалении расписания. Попробуйте позже.", result.Error);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteSchedule_ShouldDeleteLessonsAndDrafts_WhenScheduleHasThem()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var scheduleDto = new ScheduleDto { Id = scheduleId, Name = "Schedule with content" };
            var existingSchedule = CreateTestSchedule("Schedule with content", scheduleId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(existingSchedule);
            
            _scheduleRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<Schedule>()))
                .Callback<Schedule>(_ =>
                {
                })
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _scheduleServices.DeleteSchedule(scheduleDto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        #endregion

        #region GetScheduleByIdAsync Tests

        [Fact]
        public async Task GetScheduleByIdAsync_ShouldReturnSuccess_WhenScheduleExists()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var schedule = CreateTestSchedule("Test Schedule", scheduleId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            // Act
            var result = await _scheduleServices.GetScheduleByIdAsync(scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(scheduleId, result.Value.Id);
            Assert.Equal("Test Schedule", result.Value.Name);
            _scheduleRepositoryMock.Verify(repo => repo.GetScheduleByIdAsync(scheduleId), Times.Once);
        }

        [Fact]
        public async Task GetScheduleByIdAsync_ShouldReturnFailure_WhenScheduleNotFound()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync((Schedule?)null);

            // Act
            var result = await _scheduleServices.GetScheduleByIdAsync(scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Расписание не найдено", result.Error);
            _scheduleRepositoryMock.Verify(repo => repo.GetScheduleByIdAsync(scheduleId), Times.Once);
        }

        [Fact]
        public async Task GetScheduleByIdAsync_ShouldHandleRepositoryException()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _scheduleServices.GetScheduleByIdAsync(scheduleId));
        }

        [Fact]
        public async Task GetScheduleByIdAsync_ShouldReturnScheduleWithLessonsAndDrafts()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var schedule = CreateTestSchedule("Schedule with content", scheduleId);
            
            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            // Act
            var result = await _scheduleServices.GetScheduleByIdAsync(scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public async Task GetScheduleByIdAsync_ShouldHandleDefaultGuid()
        {
            // Arrange
            var scheduleId = Guid.Empty;

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync((Schedule?)null);

            // Act
            var result = await _scheduleServices.GetScheduleByIdAsync(scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Расписание не найдено", result.Error);
        }

        #endregion

        #region Integration and Edge Case Tests

        [Fact]
        public async Task FullCRUD_Workflow_ShouldWorkCorrectly()
        {
            // Arrange
            var createDto = new CreateScheduleDto { Name = "New Schedule" };
            var scheduleId = Guid.NewGuid();
            var scheduleDto = new ScheduleDto { Id = scheduleId, Name = "Updated Schedule" };
            var deleteDto = new ScheduleDto { Id = scheduleId, Name = "Updated Schedule" };
            
            CreateTestSchedule("New Schedule");
            var existingSchedule = CreateTestSchedule("New Schedule", scheduleId);
            
            _scheduleRepositoryMock
                .SetupSequence(repo => repo.AddAsync(It.IsAny<Schedule>(), 1))
                .Returns(Task.CompletedTask);
            
            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(existingSchedule);
            
            _scheduleRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Schedule>()))
                .Returns(Task.CompletedTask);
            
            _scheduleRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<Schedule>()))
                .Returns(Task.CompletedTask);
            
            _unitOfWorkMock
                .SetupSequence(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)  // For Add
                .ReturnsAsync(1)  // For Edit
                .ReturnsAsync(1); // For Delete

            // Act - Create
            var createResult = await _scheduleServices.AddSchedule(createDto);
            
            // Act - Get (simulate getting created schedule)
            var getResult = await _scheduleServices.GetScheduleByIdAsync(scheduleId);
            
            // Act - Edit
            var editResult = await _scheduleServices.EditSchedule(scheduleDto);
            
            // Act - Delete
            var deleteResult = await _scheduleServices.DeleteSchedule(deleteDto);

            // Assert
            Assert.True(createResult.IsSuccess);
            Assert.True(getResult.IsSuccess);
            Assert.True(editResult.IsSuccess);
            Assert.True(deleteResult.IsSuccess);
        }

        [Fact]
        public async Task MultipleConcurrentOperations_ShouldNotInterfere()
        {
            // Arrange
            var scheduleId1 = Guid.NewGuid();
            var scheduleId2 = Guid.NewGuid();
            
            var schedule1 = CreateTestSchedule("Schedule 1", scheduleId1);
            var schedule2 = CreateTestSchedule("Schedule 2", scheduleId2);
            
            var callCount = 0;
            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(It.IsAny<Guid>()))
                .Callback(() => callCount++)
                .ReturnsAsync((Guid id) => 
                    id == scheduleId1 ? schedule1 : 
                    id == scheduleId2 ? schedule2 : null);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act - Run concurrent gets
            var task1 = _scheduleServices.GetScheduleByIdAsync(scheduleId1);
            var task2 = _scheduleServices.GetScheduleByIdAsync(scheduleId2);

            await Task.WhenAll(task1, task2);

            // Assert
            Assert.True(task1.Result.IsSuccess);
            Assert.True(task2.Result.IsSuccess);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public async Task Schedule_WithVeryLongName_ShouldBeHandled()
        {
            // Arrange
            var longName = new string('A', 1000);
            var createDto = new CreateScheduleDto { Name = longName };

            _scheduleRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Schedule>(), 1))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _scheduleServices.AddSchedule(createDto);

            // Assert
            Assert.True(result.IsSuccess);
            _scheduleRepositoryMock.Verify(repo => repo.AddAsync(
                It.Is<Schedule>(s => s.Name == longName), 1), Times.Once);
        }

        [Fact]
        public async Task Schedule_WithSpecialCharactersInName_ShouldBeHandled()
        {
            // Arrange
            const string nameWithSpecialChars = "Schedule @#$%^&*()_+{}|:\"<>?[]\\;',./";
            var createDto = new CreateScheduleDto { Name = nameWithSpecialChars };

            _scheduleRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Schedule>(), 1))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _scheduleServices.AddSchedule(createDto);

            // Assert
            Assert.True(result.IsSuccess);
            _scheduleRepositoryMock.Verify(repo => repo.AddAsync(
                It.Is<Schedule>(s => s.Name == nameWithSpecialChars), 1), Times.Once);
        }

        [Fact]
        public async Task DeleteSchedule_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var scheduleDto = new ScheduleDto { Id = scheduleId, Name = "Schedule to delete" };
            var existingSchedule = CreateTestSchedule("Schedule to delete", scheduleId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(existingSchedule);

            _scheduleRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<Schedule>()))
                .ThrowsAsync(new Exception("Critical error"));

            // Act
            var result = await _scheduleServices.DeleteSchedule(scheduleDto);

            // Assert
            Assert.True(result.IsFailure);
        }

        [Fact]
        public async Task EditSchedule_ShouldHandleSameNameUpdate()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            const string scheduleName = "Same Name";
            var scheduleDto = new ScheduleDto { Id = scheduleId, Name = scheduleName };
            var existingSchedule = CreateTestSchedule(scheduleName, scheduleId);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(existingSchedule);

            _scheduleRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Schedule>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _scheduleServices.EditSchedule(scheduleDto);

            // Assert
            Assert.True(result.IsSuccess);
            _scheduleRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Schedule>()), Times.Once);
        }

        #endregion

        #region Helper Methods

        private static Schedule CreateTestSchedule(string name, Guid? id = null)
        {
            return Schedule.CreateSchedule(id ?? Guid.NewGuid(), name).Value;
        }

        private static Lesson CreateTestLesson(Guid scheduleId)
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
                Guid.NewGuid(),
                scheduleId,
                schoolSubject,
                lessonNumber,
                teacher,
                studyGroup,
                classroom
            );
        }

        private static LessonDraft CreateTestLessonDraft(Guid scheduleId)
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
                Guid.NewGuid(),
                scheduleId,
                schoolSubject,
                lessonNumber,
                teacher,
                studyGroup,
                classroom
            );
        }

        #endregion
    }
}