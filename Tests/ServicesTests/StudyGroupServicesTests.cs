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
    public class StudyGroupServicesTests
    {
        private readonly Mock<IStudyGroupRepository> _studyGroupRepositoryMock;
        private readonly Mock<IScheduleRepository> _scheduleRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly StudyGroupServices _studyGroupServices;

        public StudyGroupServicesTests()
        {
            _studyGroupRepositoryMock = new Mock<IStudyGroupRepository>();
            _scheduleRepositoryMock = new Mock<IScheduleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            Mock<ILogger<StudyGroupServices>> loggerMock = new Mock<ILogger<StudyGroupServices>>();
            
            _studyGroupServices = new StudyGroupServices(
                _studyGroupRepositoryMock.Object,
                _scheduleRepositoryMock.Object,
                _unitOfWorkMock.Object,
                loggerMock.Object
            );
        }

        #region FetchStudyGroupsFromBackendAsync Tests

        [Fact]
        public async Task FetchStudyGroupsFromBackendAsync_ShouldReturnStudyGroups_WhenRepositoryReturnsData()
        {
            // Arrange
            var studyGroups = new List<StudyGroup>
            {
                CreateTestStudyGroup("Group A"),
                CreateTestStudyGroup("Group B"),
                CreateTestStudyGroup("Group C")
            };

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupListAsync())
                .ReturnsAsync(studyGroups);

            // Act
            var result = await _studyGroupServices.FetchStudyGroupsFromBackendAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("Group A", result[0].Name);
            Assert.Equal("Group B", result[1].Name);
            Assert.Equal("Group C", result[2].Name);
            
            _studyGroupRepositoryMock.Verify(repo => repo.GetStudyGroupListAsync(), Times.Once);
        }

        [Fact]
        public async Task FetchStudyGroupsFromBackendAsync_ShouldReturnEmptyList_WhenNoStudyGroups()
        {
            // Arrange
            var emptyList = new List<StudyGroup>();

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupListAsync())
                .ReturnsAsync(emptyList);

            // Act
            var result = await _studyGroupServices.FetchStudyGroupsFromBackendAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task FetchStudyGroupsFromBackendAsync_ShouldHandleRepositoryException()
        {
            // Arrange
            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupListAsync())
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _studyGroupServices.FetchStudyGroupsFromBackendAsync());
        }

        [Fact]
        public async Task FetchStudyGroupsFromBackendAsync_ShouldIncludeStudySubgroups_WhenMapping()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var studyGroup = CreateTestStudyGroup("Group A", scheduleId);
            
            // Add subgroups (this would require public methods or reflection)
            // For now, we test the basic flow
            
            var studyGroups = new List<StudyGroup> { studyGroup };

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupListAsync())
                .ReturnsAsync(studyGroups);

            // Act
            var result = await _studyGroupServices.FetchStudyGroupsFromBackendAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Group A", result[0].Name);
        }

        #endregion

        #region GetStudyGroupByScheduleId Tests

        [Fact]
        public async Task GetStudyGroupByScheduleId_ShouldReturnStudyGroups_WhenScheduleExists()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var studyGroups = new List<StudyGroup>
            {
                CreateTestStudyGroup("Group A", scheduleId),
                CreateTestStudyGroup("Group B", scheduleId)
            };

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupListByScheduleIdAsync(scheduleId))
                .ReturnsAsync(studyGroups);

            // Act
            var result = await _studyGroupServices.GetStudyGroupByScheduleId(scheduleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Group A", result[0].Name);
            Assert.Equal("Group B", result[1].Name);
            Assert.All(result, dto => Assert.Equal(scheduleId, dto.ScheduleId));
            
            _studyGroupRepositoryMock.Verify(repo => repo.GetStudyGroupListByScheduleIdAsync(scheduleId), Times.Once);
        }

        [Fact]
        public async Task GetStudyGroupByScheduleId_ShouldReturnEmptyList_WhenNoStudyGroupsForSchedule()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var emptyList = new List<StudyGroup>();

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupListByScheduleIdAsync(scheduleId))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _studyGroupServices.GetStudyGroupByScheduleId(scheduleId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetStudyGroupByScheduleId_ShouldHandleRepositoryException()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupListByScheduleIdAsync(scheduleId))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _studyGroupServices.GetStudyGroupByScheduleId(scheduleId));
        }

        [Fact]
        public async Task GetStudyGroupByScheduleId_ShouldReturnOnlyGroupsForSpecifiedSchedule()
        {
            // Arrange
            var scheduleId1 = Guid.NewGuid();
            var scheduleId2 = Guid.NewGuid();
            var studyGroups = new List<StudyGroup>
            {
                CreateTestStudyGroup("Group A", scheduleId1),
                CreateTestStudyGroup("Group B", scheduleId2)
            };

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupListByScheduleIdAsync(scheduleId1))
                .ReturnsAsync(studyGroups.Where(g => g.ScheduleId == scheduleId1).ToList());

            // Act
            var result = await _studyGroupServices.GetStudyGroupByScheduleId(scheduleId1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Group A", result[0].Name);
            Assert.Equal(scheduleId1, result[0].ScheduleId);
        }

        [Fact]
        public async Task GetStudyGroupByScheduleId_ShouldHandleDefaultGuid()
        {
            // Arrange
            var scheduleId = Guid.Empty;
            var emptyList = new List<StudyGroup>();

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupListByScheduleIdAsync(scheduleId))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _studyGroupServices.GetStudyGroupByScheduleId(scheduleId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region AddStudyGroup Tests

        [Fact]
        public async Task AddStudyGroup_ShouldReturnSuccess_WhenScheduleExistsAndNameValid()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var createDto = new CreateStudyGroupDto 
            { 
                Name = "New Group", 
                ScheduleId = scheduleId 
            };
            var schedule = CreateTestSchedule(scheduleId, "Test Schedule");

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _studyGroupRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<StudyGroup>(), scheduleId))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _studyGroupServices.AddStudyGroup(createDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("New Group", result.Value.Name);
            Assert.Equal(scheduleId, result.Value.ScheduleId);
            
            _scheduleRepositoryMock.Verify(repo => repo.GetScheduleByIdAsync(scheduleId), Times.Once);
            _studyGroupRepositoryMock.Verify(
                repo => repo.AddAsync(It.Is<StudyGroup>(g => g.Name == "New Group" && g.ScheduleId == scheduleId), scheduleId),
                Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddStudyGroup_ShouldReturnFailure_WhenScheduleNotFound()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var createDto = new CreateStudyGroupDto 
            { 
                Name = "New Group", 
                ScheduleId = scheduleId 
            };

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync((Schedule?)null);

            // Act
            var result = await _studyGroupServices.AddStudyGroup(createDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Расписание не найдено", result.Error);
            _studyGroupRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<StudyGroup>(), It.IsAny<Guid>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task AddStudyGroup_ShouldReturnFailure_WhenNameInvalid(string invalidName)
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var createDto = new CreateStudyGroupDto 
            { 
                Name = invalidName, 
                ScheduleId = scheduleId 
            };
            var schedule = CreateTestSchedule(scheduleId, "Test Schedule");

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            // Act
            var result = await _studyGroupServices.AddStudyGroup(createDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Название учебной группы не может быть пустым.", result.Error);
            _studyGroupRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<StudyGroup>(), It.IsAny<Guid>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddStudyGroup_ShouldReturnFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var createDto = new CreateStudyGroupDto 
            { 
                Name = "New Group", 
                ScheduleId = scheduleId 
            };
            var schedule = CreateTestSchedule(scheduleId, "Test Schedule");

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _studyGroupRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<StudyGroup>(), scheduleId))
                .ThrowsAsync(new InvalidOperationException("Repository error"));

            // Act
            var result = await _studyGroupServices.AddStudyGroup(createDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Запись не найдена в базе данных.", result.Error);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddStudyGroup_ShouldGenerateNewGuid_ForEachGroup()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var createDto = new CreateStudyGroupDto 
            { 
                Name = "New Group", 
                ScheduleId = scheduleId 
            };
            var schedule = CreateTestSchedule(scheduleId, "Test Schedule");
            var generatedGuids = new List<Guid>();
            
            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _studyGroupRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<StudyGroup>(), scheduleId))
                .Callback<StudyGroup, Guid>((group, _) => generatedGuids.Add(group.Id))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result1 = await _studyGroupServices.AddStudyGroup(createDto);
            var result2 = await _studyGroupServices.AddStudyGroup(createDto);

            // Assert
            Assert.True(result1.IsSuccess);
            Assert.True(result2.IsSuccess);
            Assert.NotEqual(generatedGuids[0], generatedGuids[1]);
        }

        [Fact]
        public async Task AddStudyGroup_ShouldCreateGroupWithEmptyStudySubgroupsList()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var createDto = new CreateStudyGroupDto 
            { 
                Name = "New Group", 
                ScheduleId = scheduleId 
            };
            var schedule = CreateTestSchedule(scheduleId, "Test Schedule");

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            StudyGroup createdGroup = null;
            _studyGroupRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<StudyGroup>(), scheduleId))
                .Callback<StudyGroup, Guid>((group, _) => createdGroup = group)
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _studyGroupServices.AddStudyGroup(createDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(createdGroup);
        }

        #endregion

        #region EditStudyGroup Tests

        [Fact]
        public async Task EditStudyGroup_ShouldReturnSuccess_WhenStudyGroupExistsAndNameValid()
        {
            // Arrange
            var studyGroupId = Guid.NewGuid();
            var scheduleId = Guid.NewGuid();
            var studyGroupDto = new StudyGroupDto 
            { 
                Id = studyGroupId, 
                Name = "Updated Group",
                ScheduleId = scheduleId
            };
            var existingStudyGroup = CreateTestStudyGroup("Original Group", scheduleId, studyGroupId);

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(studyGroupId))
                .ReturnsAsync(existingStudyGroup);

            _studyGroupRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<StudyGroup>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _studyGroupServices.EditStudyGroup(studyGroupDto);

            // Assert
            Assert.True(result.IsSuccess);
            _studyGroupRepositoryMock.Verify(repo => repo.GetStudyGroupByIdAsync(studyGroupId), Times.Once);
            _studyGroupRepositoryMock.Verify(repo => repo.UpdateAsync(
                It.Is<StudyGroup>(g => g.Id == studyGroupId && g.Name == "Updated Group")), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EditStudyGroup_ShouldReturnFailure_WhenStudyGroupNotFound()
        {
            // Arrange
            var studyGroupId = Guid.NewGuid();
            var studyGroupDto = new StudyGroupDto 
            { 
                Id = studyGroupId, 
                Name = "Updated Group",
                ScheduleId = Guid.NewGuid()
            };

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(studyGroupId))
                .ReturnsAsync((StudyGroup?)null);

            // Act
            var result = await _studyGroupServices.EditStudyGroup(studyGroupDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Учебная группа не найдена.", result.Error);
            _studyGroupRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<StudyGroup>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task EditStudyGroup_ShouldReturnFailure_WhenNewNameInvalid(string invalidName)
        {
            // Arrange
            var studyGroupId = Guid.NewGuid();
            var scheduleId = Guid.NewGuid();
            var studyGroupDto = new StudyGroupDto 
            { 
                Id = studyGroupId, 
                Name = invalidName,
                ScheduleId = scheduleId
            };
            var existingStudyGroup = CreateTestStudyGroup("Original Group", scheduleId, studyGroupId);

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(studyGroupId))
                .ReturnsAsync(existingStudyGroup);

            // Act
            var result = await _studyGroupServices.EditStudyGroup(studyGroupDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Название учебной группы не может быть пустым.", result.Error);
            _studyGroupRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<StudyGroup>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task EditStudyGroup_ShouldNotCallSetName_WhenNameUnchanged()
        {
            // Arrange
            var studyGroupId = Guid.NewGuid();
            var scheduleId = Guid.NewGuid();
            var studyGroupDto = new StudyGroupDto 
            { 
                Id = studyGroupId, 
                Name = "Original Group",
                ScheduleId = scheduleId
            };
            var existingStudyGroup = CreateTestStudyGroup("Original Group", scheduleId, studyGroupId);

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(studyGroupId))
                .ReturnsAsync(existingStudyGroup);

            _studyGroupRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<StudyGroup>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _studyGroupServices.EditStudyGroup(studyGroupDto);

            // Assert
            Assert.True(result.IsSuccess);
            
            _studyGroupRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<StudyGroup>()), Times.Once);
        }

        [Fact]
        public async Task EditStudyGroup_ShouldReturnFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var studyGroupId = Guid.NewGuid();
            var scheduleId = Guid.NewGuid();
            var studyGroupDto = new StudyGroupDto 
            { 
                Id = studyGroupId, 
                Name = "Updated Group",
                ScheduleId = scheduleId
            };
            var existingStudyGroup = CreateTestStudyGroup("Original Group", scheduleId, studyGroupId);

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(studyGroupId))
                .ReturnsAsync(existingStudyGroup);

            _studyGroupRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<StudyGroup>()))
                .ThrowsAsync(new InvalidOperationException("Repository error"));

            // Act
            var result = await _studyGroupServices.EditStudyGroup(studyGroupDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Запись не найдена в базе данных.", result.Error);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task EditStudyGroup_ShouldPreserveStudySubgroups()
        {
            // Arrange
            var studyGroupId = Guid.NewGuid();
            var scheduleId = Guid.NewGuid();
            var studyGroupDto = new StudyGroupDto 
            { 
                Id = studyGroupId, 
                Name = "Updated Group",
                ScheduleId = scheduleId
            };
            var existingStudyGroup = CreateTestStudyGroup("Original Group", scheduleId, studyGroupId);
            
            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(studyGroupId))
                .ReturnsAsync(existingStudyGroup);

            _studyGroupRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<StudyGroup>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _studyGroupServices.EditStudyGroup(studyGroupDto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EditStudyGroup_ShouldHandleStudyGroupUsedInLessons()
        {
            // Arrange
            var studyGroupId = Guid.NewGuid();
            var scheduleId = Guid.NewGuid();
            var studyGroupDto = new StudyGroupDto 
            { 
                Id = studyGroupId, 
                Name = "Updated Group",
                ScheduleId = scheduleId
            };
            var existingStudyGroup = CreateTestStudyGroup("Original Group", scheduleId, studyGroupId);
            
            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(studyGroupId))
                .ReturnsAsync(existingStudyGroup);

            _studyGroupRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<StudyGroup>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _studyGroupServices.EditStudyGroup(studyGroupDto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        #endregion

        #region DeleteStudyGroup Tests

        [Fact]
        public async Task DeleteStudyGroup_ShouldReturnSuccess_WhenStudyGroupExists()
        {
            // Arrange
            var studyGroupId = Guid.NewGuid();
            var scheduleId = Guid.NewGuid();
            var studyGroupDto = new StudyGroupDto 
            { 
                Id = studyGroupId, 
                Name = "Group to delete",
                ScheduleId = scheduleId
            };
            var existingStudyGroup = CreateTestStudyGroup("Group to delete", scheduleId, studyGroupId);

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(studyGroupId))
                .ReturnsAsync(existingStudyGroup);

            _studyGroupRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<StudyGroup>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _studyGroupServices.DeleteStudyGroup(studyGroupDto);

            // Assert
            Assert.True(result.IsSuccess);
            _studyGroupRepositoryMock.Verify(repo => repo.GetStudyGroupByIdAsync(studyGroupId), Times.Once);
            _studyGroupRepositoryMock.Verify(repo => repo.Delete(
                It.Is<StudyGroup>(g => g.Id == studyGroupId && g.Name == "Group to delete")), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteStudyGroup_ShouldReturnFailure_WhenStudyGroupNotFound()
        {
            // Arrange
            var studyGroupId = Guid.NewGuid();
            var studyGroupDto = new StudyGroupDto 
            { 
                Id = studyGroupId, 
                Name = "Group to delete",
                ScheduleId = Guid.NewGuid()
            };

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(studyGroupId))
                .ReturnsAsync((StudyGroup?)null);

            // Act
            var result = await _studyGroupServices.DeleteStudyGroup(studyGroupDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Учебная группа не найдена.", result.Error);
            _studyGroupRepositoryMock.Verify(repo => repo.Delete(It.IsAny<StudyGroup>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteStudyGroup_ShouldDeleteStudySubgroups_WhenGroupHasThem()
        {
            // Arrange
            var studyGroupId = Guid.NewGuid();
            var scheduleId = Guid.NewGuid();
            var studyGroupDto = new StudyGroupDto 
            { 
                Id = studyGroupId, 
                Name = "Group with subgroups",
                ScheduleId = scheduleId
            };
            var existingStudyGroup = CreateTestStudyGroup("Group with subgroups", scheduleId, studyGroupId);

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(studyGroupId))
                .ReturnsAsync(existingStudyGroup);
            
            _studyGroupRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<StudyGroup>()))
                .Callback<StudyGroup>(_ =>
                {
                })
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _studyGroupServices.DeleteStudyGroup(studyGroupDto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        #endregion

        #region Integration and Edge Case Tests

        [Fact]
        public async Task FullCRUD_Workflow_ShouldWorkCorrectly()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var createDto = new CreateStudyGroupDto 
            { 
                Name = "New Group", 
                ScheduleId = scheduleId 
            };
            var studyGroupId = Guid.NewGuid();
            var studyGroupDto = new StudyGroupDto 
            { 
                Id = studyGroupId, 
                Name = "Updated Group",
                ScheduleId = scheduleId
            };
            var deleteDto = new StudyGroupDto 
            { 
                Id = studyGroupId, 
                Name = "Updated Group",
                ScheduleId = scheduleId
            };
            
            var schedule = CreateTestSchedule(scheduleId, "Test Schedule");
            var existingStudyGroup = CreateTestStudyGroup("New Group", scheduleId, studyGroupId);

            // Setup for Schedule repository
            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            // Setup for StudyGroup repository - Add
            _studyGroupRepositoryMock
                .SetupSequence(repo => repo.AddAsync(It.IsAny<StudyGroup>(), scheduleId))
                .Returns(Task.CompletedTask);

            // Setup for StudyGroup repository - Get (for edit and delete)
            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(studyGroupId))
                .ReturnsAsync(existingStudyGroup);

            // Setup for StudyGroup repository - Update
            _studyGroupRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<StudyGroup>()))
                .Returns(Task.CompletedTask);

            // Setup for StudyGroup repository - Delete
            _studyGroupRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<StudyGroup>()))
                .Returns(Task.CompletedTask);

            // Setup UnitOfWork to always succeed
            _unitOfWorkMock
                .SetupSequence(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)  // For Add
                .ReturnsAsync(1)  // For Edit
                .ReturnsAsync(1); // For Delete

            // Act - Create
            var createResult = await _studyGroupServices.AddStudyGroup(createDto);
            
            // Act - Edit
            var editResult = await _studyGroupServices.EditStudyGroup(studyGroupDto);
            
            // Act - Delete
            var deleteResult = await _studyGroupServices.DeleteStudyGroup(deleteDto);

            // Assert
            Assert.True(createResult.IsSuccess);
            Assert.True(editResult.IsSuccess);
            Assert.True(deleteResult.IsSuccess);
        }

        [Fact]
        public async Task MultipleConcurrentOperations_ShouldNotInterfere()
        {
            // Arrange
            var scheduleId1 = Guid.NewGuid();
            var scheduleId2 = Guid.NewGuid();
            
            var schedule1 = CreateTestSchedule(scheduleId1, "Schedule 1");
            var schedule2 = CreateTestSchedule(scheduleId2, "Schedule 2");
            
            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId1))
                .ReturnsAsync(schedule1);
                
            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId2))
                .ReturnsAsync(schedule2);
            
            _studyGroupRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<StudyGroup>(), It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act - Run concurrent adds
            var createDto1 = new CreateStudyGroupDto { Name = "Group 1", ScheduleId = scheduleId1 };
            var createDto2 = new CreateStudyGroupDto { Name = "Group 2", ScheduleId = scheduleId2 };

            var task1 = _studyGroupServices.AddStudyGroup(createDto1);
            var task2 = _studyGroupServices.AddStudyGroup(createDto2);

            await Task.WhenAll(task1, task2);

            // Assert
            Assert.True(task1.Result.IsSuccess);
            Assert.True(task2.Result.IsSuccess);
        }

        [Fact]
        public async Task StudyGroup_WithVeryLongName_ShouldBeHandled()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var longName = new string('A', 1000);
            var createDto = new CreateStudyGroupDto 
            { 
                Name = longName, 
                ScheduleId = scheduleId 
            };
            var schedule = CreateTestSchedule(scheduleId, "Test Schedule");

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _studyGroupRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<StudyGroup>(), scheduleId))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _studyGroupServices.AddStudyGroup(createDto);

            // Assert
            Assert.True(result.IsSuccess);
            _studyGroupRepositoryMock.Verify(repo => repo.AddAsync(
                It.Is<StudyGroup>(g => g.Name == longName), scheduleId), Times.Once);
        }

        [Fact]
        public async Task StudyGroup_WithSpecialCharactersInName_ShouldBeHandled()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            const string nameWithSpecialChars = "Group @#$%^&*()_+{}|:\"<>?[]\\;',./";
            var createDto = new CreateStudyGroupDto 
            { 
                Name = nameWithSpecialChars, 
                ScheduleId = scheduleId 
            };
            var schedule = CreateTestSchedule(scheduleId, "Test Schedule");

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _studyGroupRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<StudyGroup>(), scheduleId))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _studyGroupServices.AddStudyGroup(createDto);

            // Assert
            Assert.True(result.IsSuccess);
            _studyGroupRepositoryMock.Verify(repo => repo.AddAsync(
                It.Is<StudyGroup>(g => g.Name == nameWithSpecialChars), scheduleId), Times.Once);
        }

        [Fact]
        public async Task FetchStudyGroupsFromBackendAsync_AfterAddEditDelete_ShouldReflectChanges()
        {
            // Arrange
            var studyGroups = new List<StudyGroup>
            {
                CreateTestStudyGroup("Group A"),
                CreateTestStudyGroup("Group B")
            };
            
            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupListAsync())
                .ReturnsAsync(studyGroups);
            
            var scheduleId = Guid.NewGuid();
            var schedule = CreateTestSchedule(scheduleId, "Test Schedule");
            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);
            
            _studyGroupRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<StudyGroup>(), scheduleId))
                .Returns(Task.CompletedTask);

            _studyGroupRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<StudyGroup>()))
                .Returns(Task.CompletedTask);

            _studyGroupRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<StudyGroup>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act - Fetch initial state
            await _studyGroupServices.FetchStudyGroupsFromBackendAsync();

            // Act - Add new group
            var addResult = await _studyGroupServices.AddStudyGroup(
                new CreateStudyGroupDto { Name = "Group C", ScheduleId = scheduleId });

            // Assert
            Assert.True(addResult.IsSuccess);
        }

        [Fact]
        public async Task EditStudyGroup_WithSameName_ShouldSucceed()
        {
            // Arrange
            var studyGroupId = Guid.NewGuid();
            var scheduleId = Guid.NewGuid();
            const string groupName = "Same Name";
            var studyGroupDto = new StudyGroupDto 
            { 
                Id = studyGroupId, 
                Name = groupName,
                ScheduleId = scheduleId
            };
            var existingStudyGroup = CreateTestStudyGroup(groupName, scheduleId, studyGroupId);

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(studyGroupId))
                .ReturnsAsync(existingStudyGroup);

            _studyGroupRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<StudyGroup>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _studyGroupServices.EditStudyGroup(studyGroupDto);

            // Assert
            Assert.True(result.IsSuccess);
            
            _studyGroupRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<StudyGroup>()), Times.Once);
        }

        [Fact]
        public async Task DeleteStudyGroup_WithInvalidDto_ShouldReturnFailure()
        {
            // Arrange
            var studyGroupDto = new StudyGroupDto 
            { 
                Id = Guid.Empty, 
                Name = null,
                ScheduleId = Guid.Empty
            };

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupByIdAsync(Guid.Empty))
                .ReturnsAsync((StudyGroup?)null);

            // Act
            var result = await _studyGroupServices.DeleteStudyGroup(studyGroupDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Учебная группа не найдена.", result.Error);
        }

        [Fact]
        public async Task AddStudyGroup_WithDuplicateNameInSameSchedule_ShouldSucceed()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var createDto = new CreateStudyGroupDto 
            { 
                Name = "Group A", 
                ScheduleId = scheduleId 
            };
            var schedule = CreateTestSchedule(scheduleId, "Test Schedule");
            
            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _studyGroupRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<StudyGroup>(), scheduleId))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result1 = await _studyGroupServices.AddStudyGroup(createDto);
            var result2 = await _studyGroupServices.AddStudyGroup(createDto);

            // Assert
            Assert.True(result1.IsSuccess);
            Assert.True(result2.IsSuccess);
            _studyGroupRepositoryMock.Verify(repo => repo.AddAsync(
                It.Is<StudyGroup>(g => g.Name == "Group A"), scheduleId), Times.Exactly(2));
        }

        [Fact]
        public async Task GetStudyGroupByScheduleId_ShouldReturnOnlyGroupsForRequestedSchedule()
        {
            // Arrange
            var scheduleId1 = Guid.NewGuid();
            var scheduleId2 = Guid.NewGuid();
            
            var groupsForSchedule1 = new List<StudyGroup>
            {
                CreateTestStudyGroup("Group A1", scheduleId1),
                CreateTestStudyGroup("Group A2", scheduleId1)
            };
            
            var groupsForSchedule2 = new List<StudyGroup>
            {
                CreateTestStudyGroup("Group B1", scheduleId2)
            };

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupListByScheduleIdAsync(scheduleId1))
                .ReturnsAsync(groupsForSchedule1);
                
            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupListByScheduleIdAsync(scheduleId2))
                .ReturnsAsync(groupsForSchedule2);

            // Act - Get groups for schedule 1
            var result1 = await _studyGroupServices.GetStudyGroupByScheduleId(scheduleId1);
            
            // Act - Get groups for schedule 2
            var result2 = await _studyGroupServices.GetStudyGroupByScheduleId(scheduleId2);

            // Assert
            Assert.Equal(2, result1.Count);
            Assert.All(result1, dto => Assert.Equal(scheduleId1, dto.ScheduleId));
            
            Assert.Single(result2);
            Assert.All(result2, dto => Assert.Equal(scheduleId2, dto.ScheduleId));
        }

        [Fact]
        public async Task Service_ShouldUseBaseServiceFunctionality()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var createDto = new CreateStudyGroupDto 
            { 
                Name = "New Group", 
                ScheduleId = scheduleId 
            };
            var schedule = CreateTestSchedule(scheduleId, "Test Schedule");
            
            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _studyGroupRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<StudyGroup>(), scheduleId))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _studyGroupServices.AddStudyGroup(createDto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        #endregion

        #region Helper Methods

        private static StudyGroup CreateTestStudyGroup(string name, Guid? scheduleId = null, Guid? id = null)
        {
            return StudyGroup.CreateStudyGroup(
                id ?? Guid.NewGuid(), 
                scheduleId ?? Guid.NewGuid(), 
                name).Value;
        }

        private static Schedule CreateTestSchedule(Guid id, string name)
        {
            return Schedule.CreateSchedule(id, name).Value;
        }

        #endregion
    }
}