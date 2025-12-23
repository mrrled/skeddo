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
    public class ClassroomServicesTests
    {
        private readonly Mock<IClassroomRepository> _classroomRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ClassroomServices _classroomServices;

        public ClassroomServicesTests()
        {
            _classroomRepositoryMock = new Mock<IClassroomRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            Mock<ILogger<ClassroomServices>> loggerMock = new Mock<ILogger<ClassroomServices>>();
            
            _classroomServices = new ClassroomServices(
                _classroomRepositoryMock.Object,
                _unitOfWorkMock.Object,
                loggerMock.Object
            );
        }

        #region FetchClassroomsFromBackendAsync Tests

        [Fact]
        public async Task FetchClassroomsFromBackendAsync_ShouldReturnClassrooms_WhenRepositoryReturnsData()
        {
            // Arrange
            var classrooms = new List<Classroom>
            {
                CreateTestClassroom("Room 101", "First floor"),
                CreateTestClassroom("Room 102", "First floor"),
                CreateTestClassroom("Room 201", "Second floor")
            };

            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomListAsync(1))
                .ReturnsAsync(classrooms);

            // Act
            var result = await _classroomServices.FetchClassroomsFromBackendAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("Room 101", result[0].Name);
            Assert.Equal("First floor", result[0].Description);
            Assert.Equal("Room 102", result[1].Name);
            Assert.Equal("Room 201", result[2].Name);
            
            _classroomRepositoryMock.Verify(repo => repo.GetClassroomListAsync(1), Times.Once);
        }

        [Fact]
        public async Task FetchClassroomsFromBackendAsync_ShouldReturnEmptyList_WhenNoClassrooms()
        {
            // Arrange
            var emptyList = new List<Classroom>();

            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomListAsync(1))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _classroomServices.FetchClassroomsFromBackendAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task FetchClassroomsFromBackendAsync_ShouldHandleRepositoryException()
        {
            // Arrange
            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomListAsync(1))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _classroomServices.FetchClassroomsFromBackendAsync());
        }

        [Fact]
        public async Task FetchClassroomsFromBackendAsync_ShouldReturnClassroomsInCorrectOrder()
        {
            // Arrange
            var classrooms = new List<Classroom>
            {
                CreateTestClassroom("Room 201", "Second floor"),
                CreateTestClassroom("Room 102", "First floor"),
                CreateTestClassroom("Room 101", "First floor")
            };

            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomListAsync(1))
                .ReturnsAsync(classrooms);

            // Act
            var result = await _classroomServices.FetchClassroomsFromBackendAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Room 201", result[0].Name);
            Assert.Equal("Room 102", result[1].Name);
            Assert.Equal("Room 101", result[2].Name);
        }

        [Fact]
        public async Task FetchClassroomsFromBackendAsync_ShouldUseCorrectScheduleGroupId()
        {
            // Arrange
            var classrooms = new List<Classroom>
            {
                CreateTestClassroom("Room 101", "First floor")
            };

            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomListAsync(It.IsAny<int>()))
                .ReturnsAsync(classrooms);

            // Act
            await _classroomServices.FetchClassroomsFromBackendAsync();

            // Assert
            _classroomRepositoryMock.Verify(repo => repo.GetClassroomListAsync(1), Times.Once);
            _classroomRepositoryMock.Verify(repo => repo.GetClassroomListAsync(It.Is<int>(id => id != 1)), Times.Never);
        }

        [Fact]
        public async Task FetchClassroomsFromBackendAsync_ShouldHandleNullDescriptions()
        {
            // Arrange
            var classrooms = new List<Classroom>
            {
                CreateTestClassroom("Room 101", null),
                CreateTestClassroom("Room 102", "First floor")
            };

            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomListAsync(1))
                .ReturnsAsync(classrooms);

            // Act
            var result = await _classroomServices.FetchClassroomsFromBackendAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Null(result[0].Description);
            Assert.Equal("First floor", result[1].Description);
        }

        #endregion

        #region AddClassroom Tests

        [Fact]
        public async Task AddClassroom_ShouldReturnSuccess_WhenNameValid()
        {
            // Arrange
            var createDto = new CreateClassroomDto 
            { 
                Name = "Room 101", 
                Description = "First floor, near stairs" 
            };

            _classroomRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Classroom>(), 1))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _classroomServices.AddClassroom(createDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("Room 101", result.Value.Name);
            Assert.Equal("First floor, near stairs", result.Value.Description);
            
            _classroomRepositoryMock.Verify(
                repo => repo.AddAsync(It.Is<Classroom>(c => 
                    c.Name == "Room 101" && 
                    c.Description == "First floor, near stairs"), 1),
                Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddClassroom_ShouldReturnSuccess_WhenDescriptionIsNull()
        {
            // Arrange
            var createDto = new CreateClassroomDto 
            { 
                Name = "Room 101", 
                Description = null 
            };

            _classroomRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Classroom>(), 1))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _classroomServices.AddClassroom(createDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("Room 101", result.Value.Name);
            Assert.Null(result.Value.Description);
            
            _classroomRepositoryMock.Verify(
                repo => repo.AddAsync(It.Is<Classroom>(c => 
                    c.Name == "Room 101" && 
                    c.Description == null), 1),
                Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task AddClassroom_ShouldReturnFailure_WhenNameInvalid(string invalidName)
        {
            // Arrange
            var createDto = new CreateClassroomDto 
            { 
                Name = invalidName, 
                Description = "Some description" 
            };

            // Act
            var result = await _classroomServices.AddClassroom(createDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Название класса не может быть пустым", result.Error);
            _classroomRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Classroom>(), It.IsAny<int>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddClassroom_ShouldReturnFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var createDto = new CreateClassroomDto 
            { 
                Name = "Room 101", 
                Description = "First floor" 
            };

            _classroomRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Classroom>(), 1))
                .ThrowsAsync(new InvalidOperationException("Repository error"));

            // Act
            var result = await _classroomServices.AddClassroom(createDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Запись не найдена в базе данных.", result.Error);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddClassroom_ShouldGenerateNewGuid_ForEachClassroom()
        {
            // Arrange
            var createDto = new CreateClassroomDto 
            { 
                Name = "Room 101", 
                Description = "First floor" 
            };

            var generatedGuids = new List<Guid>();
            
            _classroomRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Classroom>(), 1))
                .Callback<Classroom, int>((classroom, _) => generatedGuids.Add(classroom.Id))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result1 = await _classroomServices.AddClassroom(createDto);
            var result2 = await _classroomServices.AddClassroom(createDto);

            // Assert
            Assert.True(result1.IsSuccess);
            Assert.True(result2.IsSuccess);
            Assert.NotEqual(generatedGuids[0], generatedGuids[1]);
        }

        [Fact]
        public async Task AddClassroom_ShouldUseCorrectScheduleGroupId()
        {
            // Arrange
            var createDto = new CreateClassroomDto 
            { 
                Name = "Room 101", 
                Description = "First floor" 
            };

            _classroomRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Classroom>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _classroomServices.AddClassroom(createDto);

            // Assert
            _classroomRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Classroom>(), 1), Times.Once);
            _classroomRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Classroom>(), It.Is<int>(id => id != 1)), Times.Never);
        }

        [Fact]
        public async Task AddClassroom_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var createDto = new CreateClassroomDto 
            { 
                Name = "Room 101", 
                Description = "First floor" 
            };

            _classroomRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Classroom>(), 1))
                .ThrowsAsync(new Exception("Critical error"));

            // Act
            var result = await _classroomServices.AddClassroom(createDto);

            // Assert
            Assert.True(result.IsFailure);
        }

        #endregion

        #region EditClassroom Tests

        [Fact]
        public async Task EditClassroom_ShouldReturnSuccess_WhenClassroomExistsAndDataValid()
        {
            // Arrange
            var classroomId = Guid.NewGuid();
            var classroomDto = new ClassroomDto 
            { 
                Id = classroomId, 
                Name = "Updated Room 101", 
                Description = "Updated description" 
            };
            var existingClassroom = CreateTestClassroom("Original Room 101", "Original description", classroomId);

            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomByIdAsync(classroomId))
                .ReturnsAsync(existingClassroom);

            _classroomRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Classroom>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _classroomServices.EditClassroom(classroomDto);

            // Assert
            Assert.True(result.IsSuccess);
            _classroomRepositoryMock.Verify(repo => repo.GetClassroomByIdAsync(classroomId), Times.Once);
            _classroomRepositoryMock.Verify(repo => repo.UpdateAsync(
                It.Is<Classroom>(c => 
                    c.Id == classroomId && 
                    c.Name == "Updated Room 101" && 
                    c.Description == "Updated description")), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EditClassroom_ShouldReturnFailure_WhenClassroomNotFound()
        {
            // Arrange
            var classroomId = Guid.NewGuid();
            var classroomDto = new ClassroomDto 
            { 
                Id = classroomId, 
                Name = "Updated Room 101", 
                Description = "Updated description" 
            };

            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomByIdAsync(classroomId))
                .ReturnsAsync((Classroom?)null);

            // Act
            var result = await _classroomServices.EditClassroom(classroomDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Аудитория не найдена.", result.Error);
            _classroomRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Classroom>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task EditClassroom_ShouldReturnFailure_WhenNewNameInvalid(string invalidName)
        {
            // Arrange
            var classroomId = Guid.NewGuid();
            var classroomDto = new ClassroomDto 
            { 
                Id = classroomId, 
                Name = invalidName, 
                Description = "Some description" 
            };
            var existingClassroom = CreateTestClassroom("Original Room 101", "Original description", classroomId);

            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomByIdAsync(classroomId))
                .ReturnsAsync(existingClassroom);

            // Act
            var result = await _classroomServices.EditClassroom(classroomDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Название класса не может быть пустым", result.Error);
            _classroomRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Classroom>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task EditClassroom_ShouldUpdateDescription_EvenIfNameUnchanged()
        {
            // Arrange
            var classroomId = Guid.NewGuid();
            var classroomDto = new ClassroomDto 
            { 
                Id = classroomId, 
                Name = "Room 101", 
                Description = "Updated description" 
            };
            var existingClassroom = CreateTestClassroom("Room 101", "Original description", classroomId);

            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomByIdAsync(classroomId))
                .ReturnsAsync(existingClassroom);

            _classroomRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Classroom>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _classroomServices.EditClassroom(classroomDto);

            // Assert
            Assert.True(result.IsSuccess);
            _classroomRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Classroom>()), Times.Once);
        }

        [Fact]
        public async Task EditClassroom_ShouldUpdateName_WhenNameChanged()
        {
            // Arrange
            var classroomId = Guid.NewGuid();
            var classroomDto = new ClassroomDto 
            { 
                Id = classroomId, 
                Name = "Room 102", 
                Description = "Same description" 
            };
            var existingClassroom = CreateTestClassroom("Room 101", "Same description", classroomId);

            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomByIdAsync(classroomId))
                .ReturnsAsync(existingClassroom);

            _classroomRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Classroom>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _classroomServices.EditClassroom(classroomDto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EditClassroom_ShouldNotUpdateDescription_WhenDescriptionUnchanged()
        {
            // Arrange
            var classroomId = Guid.NewGuid();
            var classroomDto = new ClassroomDto 
            { 
                Id = classroomId, 
                Name = "Room 101", 
                Description = "Same description" 
            };
            var existingClassroom = CreateTestClassroom("Room 101", "Same description", classroomId);

            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomByIdAsync(classroomId))
                .ReturnsAsync(existingClassroom);

            _classroomRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Classroom>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _classroomServices.EditClassroom(classroomDto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EditClassroom_ShouldHandleNullToEmptyDescription()
        {
            // Arrange
            var classroomId = Guid.NewGuid();
            var classroomDto = new ClassroomDto 
            { 
                Id = classroomId, 
                Name = "Room 101", 
                Description = "" 
            };
            var existingClassroom = CreateTestClassroom("Room 101", null, classroomId);

            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomByIdAsync(classroomId))
                .ReturnsAsync(existingClassroom);

            _classroomRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Classroom>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _classroomServices.EditClassroom(classroomDto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EditClassroom_ShouldHandleEmptyToNullDescription()
        {
            // Arrange
            var classroomId = Guid.NewGuid();
            var classroomDto = new ClassroomDto 
            { 
                Id = classroomId, 
                Name = "Room 101", 
                Description = null 
            };
            var existingClassroom = CreateTestClassroom("Room 101", "", classroomId);

            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomByIdAsync(classroomId))
                .ReturnsAsync(existingClassroom);

            _classroomRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Classroom>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _classroomServices.EditClassroom(classroomDto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EditClassroom_ShouldReturnFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var classroomId = Guid.NewGuid();
            var classroomDto = new ClassroomDto 
            { 
                Id = classroomId, 
                Name = "Updated Room 101", 
                Description = "Updated description" 
            };
            var existingClassroom = CreateTestClassroom("Original Room 101", "Original description", classroomId);

            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomByIdAsync(classroomId))
                .ReturnsAsync(existingClassroom);

            _classroomRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Classroom>()))
                .ThrowsAsync(new InvalidOperationException("Repository error"));

            // Act
            var result = await _classroomServices.EditClassroom(classroomDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Запись не найдена в базе данных.", result.Error);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task EditClassroom_ShouldHandleClassroomUsedInLessons()
        {
            // Arrange
            var classroomId = Guid.NewGuid();
            var classroomDto = new ClassroomDto 
            { 
                Id = classroomId, 
                Name = "Updated Room 101", 
                Description = "Updated description" 
            };
            var existingClassroom = CreateTestClassroom("Original Room 101", "Original description", classroomId);
            
            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomByIdAsync(classroomId))
                .ReturnsAsync(existingClassroom);

            _classroomRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Classroom>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _classroomServices.EditClassroom(classroomDto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        #endregion

        #region DeleteClassroom Tests

        [Fact]
        public async Task DeleteClassroom_ShouldReturnSuccess_WhenClassroomExists()
        {
            // Arrange
            var classroomId = Guid.NewGuid();
            var classroomDto = new ClassroomDto 
            { 
                Id = classroomId, 
                Name = "Room 101", 
                Description = "First floor" 
            };
            var existingClassroom = CreateTestClassroom("Room 101", "First floor", classroomId);

            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomByIdAsync(classroomId))
                .ReturnsAsync(existingClassroom);

            _classroomRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<Classroom>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _classroomServices.DeleteClassroom(classroomDto);

            // Assert
            Assert.True(result.IsSuccess);
            _classroomRepositoryMock.Verify(repo => repo.GetClassroomByIdAsync(classroomId), Times.Once);
            _classroomRepositoryMock.Verify(repo => repo.Delete(
                It.Is<Classroom>(c => c.Id == classroomId && c.Name == "Room 101")), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteClassroom_ShouldReturnFailure_WhenClassroomNotFound()
        {
            // Arrange
            var classroomId = Guid.NewGuid();
            var classroomDto = new ClassroomDto 
            { 
                Id = classroomId, 
                Name = "Room 101", 
                Description = "First floor" 
            };

            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomByIdAsync(classroomId))
                .ReturnsAsync((Classroom?)null);

            // Act
            var result = await _classroomServices.DeleteClassroom(classroomDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Аудитория не найдена.", result.Error);
            _classroomRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Classroom>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteClassroom_ShouldHandleClassroomWithNullDescription()
        {
            // Arrange
            var classroomId = Guid.NewGuid();
            var classroomDto = new ClassroomDto 
            { 
                Id = classroomId, 
                Name = "Room 101", 
                Description = null 
            };
            var existingClassroom = CreateTestClassroom("Room 101", null, classroomId);

            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomByIdAsync(classroomId))
                .ReturnsAsync(existingClassroom);

            _classroomRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<Classroom>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _classroomServices.DeleteClassroom(classroomDto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        #endregion

        #region Integration and Edge Case Tests

        [Fact]
        public async Task FullCRUD_Workflow_ShouldWorkCorrectly()
        {
            // Arrange
            var createDto = new CreateClassroomDto 
            { 
                Name = "New Room", 
                Description = "New description" 
            };
            var classroomId = Guid.NewGuid();
            var classroomDto = new ClassroomDto 
            { 
                Id = classroomId, 
                Name = "Updated Room", 
                Description = "Updated description" 
            };
            var deleteDto = new ClassroomDto 
            { 
                Id = classroomId, 
                Name = "Updated Room", 
                Description = "Updated description" 
            };
            
            var existingClassroom = CreateTestClassroom("New Room", "New description", classroomId);

            // Setup for Add
            _classroomRepositoryMock
                .SetupSequence(repo => repo.AddAsync(It.IsAny<Classroom>(), 1))
                .Returns(Task.CompletedTask);

            // Setup for Get (for edit and delete)
            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomByIdAsync(classroomId))
                .ReturnsAsync(existingClassroom);

            // Setup for Update
            _classroomRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Classroom>()))
                .Returns(Task.CompletedTask);

            // Setup for Delete
            _classroomRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<Classroom>()))
                .Returns(Task.CompletedTask);

            // Setup UnitOfWork to always succeed
            _unitOfWorkMock
                .SetupSequence(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)  // For Add
                .ReturnsAsync(1)  // For Edit
                .ReturnsAsync(1); // For Delete

            // Act - Create
            var createResult = await _classroomServices.AddClassroom(createDto);
            
            // Act - Edit
            var editResult = await _classroomServices.EditClassroom(classroomDto);
            
            // Act - Delete
            var deleteResult = await _classroomServices.DeleteClassroom(deleteDto);

            // Assert
            Assert.True(createResult.IsSuccess);
            Assert.True(editResult.IsSuccess);
            Assert.True(deleteResult.IsSuccess);
        }

        [Fact]
        public async Task MultipleConcurrentOperations_ShouldNotInterfere()
        {
            // Arrange
            var classroomId1 = Guid.NewGuid();
            var classroomId2 = Guid.NewGuid();
            
            var classroom1 = CreateTestClassroom("Room 101", "First floor", classroomId1);
            var classroom2 = CreateTestClassroom("Room 102", "First floor", classroomId2);

            // Setup repository to handle concurrent calls
            var callCount = 0;
            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomByIdAsync(It.IsAny<Guid>()))
                .Callback(() => callCount++)
                .ReturnsAsync((Guid id) => 
                    id == classroomId1 ? classroom1 : 
                    id == classroomId2 ? classroom2 : null);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act - Run concurrent edits
            var editDto1 = new ClassroomDto { Id = classroomId1, Name = "Updated 101", Description = "Updated" };
            var editDto2 = new ClassroomDto { Id = classroomId2, Name = "Updated 102", Description = "Updated" };

            var task1 = _classroomServices.EditClassroom(editDto1);
            var task2 = _classroomServices.EditClassroom(editDto2);

            await Task.WhenAll(task1, task2);

            // Assert
            Assert.True(task1.Result.IsSuccess);
            Assert.True(task2.Result.IsSuccess);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public async Task Classroom_WithVeryLongNameAndDescription_ShouldBeHandled()
        {
            // Arrange
            var longName = new string('A', 1000); 
            var longDescription = new string('B', 2000); 
            var createDto = new CreateClassroomDto 
            { 
                Name = longName, 
                Description = longDescription 
            };

            _classroomRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Classroom>(), 1))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _classroomServices.AddClassroom(createDto);

            // Assert
            Assert.True(result.IsSuccess);
            _classroomRepositoryMock.Verify(repo => repo.AddAsync(
                It.Is<Classroom>(c => c.Name == longName && c.Description == longDescription), 1), Times.Once);
        }

        [Fact]
        public async Task Classroom_WithSpecialCharactersInName_ShouldBeHandled()
        {
            // Arrange
            var nameWithSpecialChars = "Room @#$%^&*()_+{}|:\"<>?[]\\;',./";
            var descriptionWithSpecialChars = "Description @#$%^&*()_+{}|:\"<>?[]\\;',./";
            var createDto = new CreateClassroomDto 
            { 
                Name = nameWithSpecialChars, 
                Description = descriptionWithSpecialChars 
            };

            _classroomRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Classroom>(), 1))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _classroomServices.AddClassroom(createDto);

            // Assert
            Assert.True(result.IsSuccess);
            _classroomRepositoryMock.Verify(repo => repo.AddAsync(
                It.Is<Classroom>(c => 
                    c.Name == nameWithSpecialChars && 
                    c.Description == descriptionWithSpecialChars), 1), Times.Once);
        }

        [Fact]
        public async Task FetchClassroomsFromBackendAsync_AfterAddEditDelete_ShouldReflectChanges()
        {
            // Arrange
            var classrooms = new List<Classroom>
            {
                CreateTestClassroom("Room 101", "First floor"),
                CreateTestClassroom("Room 102", "First floor")
            };

            // Setup for fetch
            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomListAsync(1))
                .ReturnsAsync(classrooms);

            // Setup for other operations
            _classroomRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Classroom>(), 1))
                .Returns(Task.CompletedTask);

            _classroomRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Classroom>()))
                .Returns(Task.CompletedTask);

            _classroomRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<Classroom>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act - Fetch initial state
            await _classroomServices.FetchClassroomsFromBackendAsync();

            // Act - Add new classroom
            var addResult = await _classroomServices.AddClassroom(
                new CreateClassroomDto { Name = "Room 103", Description = "First floor" });

            // Assert
            Assert.True(addResult.IsSuccess);
        }

        [Fact]
        public async Task EditClassroom_WithSameData_ShouldSucceed()
        {
            // Arrange
            var classroomId = Guid.NewGuid();
            const string classroomName = "Room 101";
            const string classroomDescription = "First floor";
            var classroomDto = new ClassroomDto 
            { 
                Id = classroomId, 
                Name = classroomName, 
                Description = classroomDescription 
            };
            var existingClassroom = CreateTestClassroom(classroomName, classroomDescription, classroomId);

            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomByIdAsync(classroomId))
                .ReturnsAsync(existingClassroom);

            _classroomRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Classroom>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _classroomServices.EditClassroom(classroomDto);

            // Assert
            Assert.True(result.IsSuccess);
            // Even with same data, the update should proceed
            _classroomRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Classroom>()), Times.Once);
        }

        [Fact]
        public async Task DeleteClassroom_WithInvalidDto_ShouldReturnFailure()
        {
            // Arrange
            var classroomDto = new ClassroomDto 
            { 
                Id = Guid.Empty, 
                Name = null, 
                Description = null 
            };

            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomByIdAsync(Guid.Empty))
                .ReturnsAsync((Classroom?)null);

            // Act
            var result = await _classroomServices.DeleteClassroom(classroomDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Аудитория не найдена.", result.Error);
        }

        [Fact]
        public async Task AddClassroom_WithDuplicateName_ShouldSucceed()
        {
            // Arrange
            var createDto = new CreateClassroomDto 
            { 
                Name = "Room 101", 
                Description = "First floor" 
            };
            
            _classroomRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Classroom>(), 1))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result1 = await _classroomServices.AddClassroom(createDto);
            var result2 = await _classroomServices.AddClassroom(createDto);

            // Assert
            Assert.True(result1.IsSuccess);
            Assert.True(result2.IsSuccess);
            _classroomRepositoryMock.Verify(repo => repo.AddAsync(
                It.Is<Classroom>(c => c.Name == "Room 101"), 1), Times.Exactly(2));
        }

        [Fact]
        public async Task Service_ShouldUseBaseServiceFunctionality()
        {
            // Arrange
            var createDto = new CreateClassroomDto 
            { 
                Name = "Room 101", 
                Description = "First floor" 
            };
            
            _classroomRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Classroom>(), 1))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _classroomServices.AddClassroom(createDto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EditClassroom_ShouldHandleSpacesInNameCorrectly()
        {
            // Arrange
            var classroomId = Guid.NewGuid();
            var classroomDto = new ClassroomDto 
            { 
                Id = classroomId, 
                Name = "  Room 101  ", 
                Description = "Description" 
            };
            var existingClassroom = CreateTestClassroom("Room 101", "Description", classroomId);

            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomByIdAsync(classroomId))
                .ReturnsAsync(existingClassroom);

            _classroomRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Classroom>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _classroomServices.EditClassroom(classroomDto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EditClassroom_ShouldHandleNameChangeFromValidToValid()
        {
            var classroomId = Guid.NewGuid();
            var classroomDto = new ClassroomDto 
            { 
                Id = classroomId, 
                Name = "New Room Name", 
                Description = "Description" 
            };
            var existingClassroom = CreateTestClassroom("Old Room Name", "Description", classroomId);

            _classroomRepositoryMock
                .Setup(repo => repo.GetClassroomByIdAsync(classroomId))
                .ReturnsAsync(existingClassroom);

            _classroomRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Classroom>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _classroomServices.EditClassroom(classroomDto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        #endregion

        #region Helper Methods

        private static Classroom CreateTestClassroom(string name, string? description, Guid? id = null)
        {
            return Classroom.CreateClassroom(id ?? Guid.NewGuid(), name, description).Value;
        }

        #endregion
    }
}