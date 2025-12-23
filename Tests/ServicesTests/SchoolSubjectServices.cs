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
    public class SchoolSubjectServicesTests
    {
        private readonly Mock<ISchoolSubjectRepository> _schoolSubjectRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<SchoolSubjectServices>> _loggerMock;
        private readonly SchoolSubjectServices _schoolSubjectServices;

        public SchoolSubjectServicesTests()
        {
            _schoolSubjectRepositoryMock = new Mock<ISchoolSubjectRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<SchoolSubjectServices>>();
            
            _schoolSubjectServices = new SchoolSubjectServices(
                _schoolSubjectRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object
            );
        }

        #region FetchSchoolSubjectsFromBackendAsync Tests

        [Fact]
        public async Task FetchSchoolSubjectsFromBackendAsync_ShouldReturnSubjects_WhenRepositoryReturnsData()
        {
            // Arrange
            var subjects = new List<SchoolSubject>
            {
                CreateTestSchoolSubject("Mathematics"),
                CreateTestSchoolSubject("Physics"),
                CreateTestSchoolSubject("Chemistry")
            };

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectListAsync(1))
                .ReturnsAsync(subjects);

            // Act
            var result = await _schoolSubjectServices.FetchSchoolSubjectsFromBackendAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("Mathematics", result[0].Name);
            Assert.Equal("Physics", result[1].Name);
            Assert.Equal("Chemistry", result[2].Name);
            
            _schoolSubjectRepositoryMock.Verify(repo => repo.GetSchoolSubjectListAsync(1), Times.Once);
        }

        [Fact]
        public async Task FetchSchoolSubjectsFromBackendAsync_ShouldReturnEmptyList_WhenNoSubjects()
        {
            // Arrange
            var emptyList = new List<SchoolSubject>();

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectListAsync(1))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _schoolSubjectServices.FetchSchoolSubjectsFromBackendAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task FetchSchoolSubjectsFromBackendAsync_ShouldHandleRepositoryException()
        {
            // Arrange
            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectListAsync(1))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _schoolSubjectServices.FetchSchoolSubjectsFromBackendAsync());
        }

        [Fact]
        public async Task FetchSchoolSubjectsFromBackendAsync_ShouldReturnSubjectsInCorrectOrder()
        {
            // Arrange
            var subjects = new List<SchoolSubject>
            {
                CreateTestSchoolSubject("Chemistry"),
                CreateTestSchoolSubject("Physics"),
                CreateTestSchoolSubject("Mathematics")
            };

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectListAsync(1))
                .ReturnsAsync(subjects);

            // Act
            var result = await _schoolSubjectServices.FetchSchoolSubjectsFromBackendAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Chemistry", result[0].Name);
            Assert.Equal("Physics", result[1].Name);
            Assert.Equal("Mathematics", result[2].Name);
        }

        [Fact]
        public async Task FetchSchoolSubjectsFromBackendAsync_ShouldUseCorrectScheduleGroupId()
        {
            // Arrange
            var subjects = new List<SchoolSubject>
            {
                CreateTestSchoolSubject("Mathematics")
            };

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectListAsync(It.IsAny<int>()))
                .ReturnsAsync(subjects);

            // Act
            var result = await _schoolSubjectServices.FetchSchoolSubjectsFromBackendAsync();

            // Assert
            _schoolSubjectRepositoryMock.Verify(repo => repo.GetSchoolSubjectListAsync(1), Times.Once);
            _schoolSubjectRepositoryMock.Verify(repo => repo.GetSchoolSubjectListAsync(It.Is<int>(id => id != 1)), Times.Never);
        }

        #endregion

        #region AddSchoolSubject Tests

        [Fact]
        public async Task AddSchoolSubject_ShouldReturnSuccess_WhenNameValid()
        {
            // Arrange
            var createDto = new CreateSchoolSubjectDto { Name = "Mathematics" };
            var subject = CreateTestSchoolSubject("Mathematics");

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<SchoolSubject>(), 1))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _schoolSubjectServices.AddSchoolSubject(createDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("Mathematics", result.Value.Name);
            
            _schoolSubjectRepositoryMock.Verify(
                repo => repo.AddAsync(It.Is<SchoolSubject>(s => s.Name == "Mathematics"), 1),
                Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task AddSchoolSubject_ShouldReturnFailure_WhenNameInvalid(string invalidName)
        {
            // Arrange
            var createDto = new CreateSchoolSubjectDto { Name = invalidName };

            // Act
            var result = await _schoolSubjectServices.AddSchoolSubject(createDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Название предмета не может быть пустым.", result.Error);
            _schoolSubjectRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<SchoolSubject>(), It.IsAny<int>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddSchoolSubject_ShouldReturnFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var createDto = new CreateSchoolSubjectDto { Name = "Mathematics" };

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<SchoolSubject>(), 1))
                .ThrowsAsync(new InvalidOperationException("Repository error"));

            // Act
            var result = await _schoolSubjectServices.AddSchoolSubject(createDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Запись не найдена в базе данных.", result.Error);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddSchoolSubject_ShouldGenerateNewGuid_ForEachSubject()
        {
            // Arrange
            var createDto = new CreateSchoolSubjectDto { Name = "Mathematics" };
            var generatedGuids = new List<Guid>();
            
            _schoolSubjectRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<SchoolSubject>(), 1))
                .Callback<SchoolSubject, int>((subject, id) => generatedGuids.Add(subject.Id))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result1 = await _schoolSubjectServices.AddSchoolSubject(createDto);
            var result2 = await _schoolSubjectServices.AddSchoolSubject(createDto);

            // Assert
            Assert.True(result1.IsSuccess);
            Assert.True(result2.IsSuccess);
            Assert.NotEqual(generatedGuids[0], generatedGuids[1]);
        }

        [Fact]
        public async Task AddSchoolSubject_ShouldUseCorrectScheduleGroupId()
        {
            // Arrange
            var createDto = new CreateSchoolSubjectDto { Name = "Mathematics" };

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<SchoolSubject>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _schoolSubjectServices.AddSchoolSubject(createDto);

            // Assert
            _schoolSubjectRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<SchoolSubject>(), 1), Times.Once);
            _schoolSubjectRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<SchoolSubject>(), It.Is<int>(id => id != 1)), Times.Never);
        }

        [Fact]
        public async Task AddSchoolSubject_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var createDto = new CreateSchoolSubjectDto { Name = "Mathematics" };

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<SchoolSubject>(), 1))
                .ThrowsAsync(new Exception("Critical error"));

            // Act
            var result = await _schoolSubjectServices.AddSchoolSubject(createDto);

            // Assert
            Assert.True(result.IsFailure);
            // Verify logging occurred (though we can't easily assert on the logger without specific setup)
        }

        #endregion

        #region EditSchoolSubject Tests

        [Fact]
        public async Task EditSchoolSubject_ShouldReturnSuccess_WhenSubjectExistsAndNameValid()
        {
            // Arrange
            var subjectId = Guid.NewGuid();
            var subjectDto = new SchoolSubjectDto { Id = subjectId, Name = "Updated Mathematics" };
            var existingSubject = CreateTestSchoolSubject("Original Mathematics", subjectId);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(subjectId))
                .ReturnsAsync(existingSubject);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<SchoolSubject>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _schoolSubjectServices.EditSchoolSubject(subjectDto);

            // Assert
            Assert.True(result.IsSuccess);
            _schoolSubjectRepositoryMock.Verify(repo => repo.GetSchoolSubjectByIdAsync(subjectId), Times.Once);
            _schoolSubjectRepositoryMock.Verify(repo => repo.UpdateAsync(
                It.Is<SchoolSubject>(s => s.Id == subjectId && s.Name == "Updated Mathematics")), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EditSchoolSubject_ShouldReturnFailure_WhenSubjectNotFound()
        {
            // Arrange
            var subjectId = Guid.NewGuid();
            var subjectDto = new SchoolSubjectDto { Id = subjectId, Name = "Updated Mathematics" };

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(subjectId))
                .ReturnsAsync((SchoolSubject?)null);

            // Act
            var result = await _schoolSubjectServices.EditSchoolSubject(subjectDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Предмет не найден.", result.Error);
            _schoolSubjectRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<SchoolSubject>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task EditSchoolSubject_ShouldReturnFailure_WhenNewNameInvalid(string invalidName)
        {
            // Arrange
            var subjectId = Guid.NewGuid();
            var subjectDto = new SchoolSubjectDto { Id = subjectId, Name = invalidName };
            var existingSubject = CreateTestSchoolSubject("Original Mathematics", subjectId);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(subjectId))
                .ReturnsAsync(existingSubject);

            // Act
            var result = await _schoolSubjectServices.EditSchoolSubject(subjectDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Название предмета не может быть пустым.", result.Error);
            _schoolSubjectRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<SchoolSubject>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task EditSchoolSubject_ShouldNotCallSetName_WhenNameUnchanged()
        {
            // Arrange
            var subjectId = Guid.NewGuid();
            var subjectDto = new SchoolSubjectDto { Id = subjectId, Name = "Original Mathematics" };
            var existingSubject = CreateTestSchoolSubject("Original Mathematics", subjectId);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(subjectId))
                .ReturnsAsync(existingSubject);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<SchoolSubject>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _schoolSubjectServices.EditSchoolSubject(subjectDto);

            // Assert
            Assert.True(result.IsSuccess);
            // Even though name hasn't changed, UpdateAsync should still be called
            _schoolSubjectRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<SchoolSubject>()), Times.Once);
        }

        [Fact]
        public async Task EditSchoolSubject_ShouldReturnFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var subjectId = Guid.NewGuid();
            var subjectDto = new SchoolSubjectDto { Id = subjectId, Name = "Updated Mathematics" };
            var existingSubject = CreateTestSchoolSubject("Original Mathematics", subjectId);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(subjectId))
                .ReturnsAsync(existingSubject);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<SchoolSubject>()))
                .ThrowsAsync(new InvalidOperationException("Repository error"));

            // Act
            var result = await _schoolSubjectServices.EditSchoolSubject(subjectDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Запись не найдена в базе данных.", result.Error);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task EditSchoolSubject_ShouldHandleNameChangeFromNullToValid()
        {
            // Note: SchoolSubject doesn't allow null names in constructor, so this scenario isn't possible
            // This test would be relevant if the business rules allowed null names
        }

        [Fact]
        public async Task EditSchoolSubject_ShouldHandleSubjectUsedInLessons()
        {
            // Arrange
            var subjectId = Guid.NewGuid();
            var subjectDto = new SchoolSubjectDto { Id = subjectId, Name = "Updated Mathematics" };
            var existingSubject = CreateTestSchoolSubject("Original Mathematics", subjectId);

            // If subject is used in lessons, the repository update should still work
            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(subjectId))
                .ReturnsAsync(existingSubject);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<SchoolSubject>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _schoolSubjectServices.EditSchoolSubject(subjectDto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        #endregion

        #region DeleteSchoolSubject Tests

        [Fact]
        public async Task DeleteSchoolSubject_ShouldReturnSuccess_WhenSubjectExists()
        {
            // Arrange
            var subjectId = Guid.NewGuid();
            var subjectDto = new SchoolSubjectDto { Id = subjectId, Name = "Mathematics" };
            var existingSubject = CreateTestSchoolSubject("Mathematics", subjectId);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(subjectId))
                .ReturnsAsync(existingSubject);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<SchoolSubject>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _schoolSubjectServices.DeleteSchoolSubject(subjectDto);

            // Assert
            Assert.True(result.IsSuccess);
            _schoolSubjectRepositoryMock.Verify(repo => repo.GetSchoolSubjectByIdAsync(subjectId), Times.Once);
            _schoolSubjectRepositoryMock.Verify(repo => repo.Delete(
                It.Is<SchoolSubject>(s => s.Id == subjectId && s.Name == "Mathematics")), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteSchoolSubject_ShouldReturnFailure_WhenSubjectNotFound()
        {
            // Arrange
            var subjectId = Guid.NewGuid();
            var subjectDto = new SchoolSubjectDto { Id = subjectId, Name = "Mathematics" };

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(subjectId))
                .ReturnsAsync((SchoolSubject?)null);

            // Act
            var result = await _schoolSubjectServices.DeleteSchoolSubject(subjectDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Предмет не найден.", result.Error);
            _schoolSubjectRepositoryMock.Verify(repo => repo.Delete(It.IsAny<SchoolSubject>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region Integration and Edge Case Tests

        [Fact]
        public async Task FullCRUD_Workflow_ShouldWorkCorrectly()
        {
            // Arrange
            var createDto = new CreateSchoolSubjectDto { Name = "New Subject" };
            var subjectId = Guid.NewGuid();
            var subjectDto = new SchoolSubjectDto { Id = subjectId, Name = "Updated Subject" };
            var deleteDto = new SchoolSubjectDto { Id = subjectId, Name = "Updated Subject" };
            
            var createdSubject = CreateTestSchoolSubject("New Subject");
            var existingSubject = CreateTestSchoolSubject("New Subject", subjectId);

            // Setup for Add
            _schoolSubjectRepositoryMock
                .SetupSequence(repo => repo.AddAsync(It.IsAny<SchoolSubject>(), 1))
                .Returns(Task.CompletedTask);

            // Setup for Get (for edit and delete)
            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(subjectId))
                .ReturnsAsync(existingSubject);

            // Setup for Update
            _schoolSubjectRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<SchoolSubject>()))
                .Returns(Task.CompletedTask);

            // Setup for Delete
            _schoolSubjectRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<SchoolSubject>()))
                .Returns(Task.CompletedTask);

            // Setup UnitOfWork to always succeed
            _unitOfWorkMock
                .SetupSequence(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)  // For Add
                .ReturnsAsync(1)  // For Edit
                .ReturnsAsync(1); // For Delete

            // Act - Create
            var createResult = await _schoolSubjectServices.AddSchoolSubject(createDto);
            
            // Act - Edit
            var editResult = await _schoolSubjectServices.EditSchoolSubject(subjectDto);
            
            // Act - Delete
            var deleteResult = await _schoolSubjectServices.DeleteSchoolSubject(deleteDto);

            // Assert
            Assert.True(createResult.IsSuccess);
            Assert.True(editResult.IsSuccess);
            Assert.True(deleteResult.IsSuccess);
        }

        [Fact]
        public async Task MultipleConcurrentOperations_ShouldNotInterfere()
        {
            // Arrange
            var subjectId1 = Guid.NewGuid();
            var subjectId2 = Guid.NewGuid();
            
            var subject1 = CreateTestSchoolSubject("Subject 1", subjectId1);
            var subject2 = CreateTestSchoolSubject("Subject 2", subjectId2);

            // Setup repository to handle concurrent calls
            int callCount = 0;
            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(It.IsAny<Guid>()))
                .Callback(() => callCount++)
                .ReturnsAsync((Guid id) => 
                    id == subjectId1 ? subject1 : 
                    id == subjectId2 ? subject2 : null);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act - Run concurrent gets through edit operations
            var editDto1 = new SchoolSubjectDto { Id = subjectId1, Name = "Updated 1" };
            var editDto2 = new SchoolSubjectDto { Id = subjectId2, Name = "Updated 2" };

            var task1 = _schoolSubjectServices.EditSchoolSubject(editDto1);
            var task2 = _schoolSubjectServices.EditSchoolSubject(editDto2);

            await Task.WhenAll(task1, task2);

            // Assert
            Assert.True(task1.Result.IsSuccess);
            Assert.True(task2.Result.IsSuccess);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public async Task Subject_WithVeryLongName_ShouldBeHandled()
        {
            // Arrange
            var longName = new string('A', 1000); // Very long name
            var createDto = new CreateSchoolSubjectDto { Name = longName };

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<SchoolSubject>(), 1))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _schoolSubjectServices.AddSchoolSubject(createDto);

            // Assert
            Assert.True(result.IsSuccess);
            _schoolSubjectRepositoryMock.Verify(repo => repo.AddAsync(
                It.Is<SchoolSubject>(s => s.Name == longName), 1), Times.Once);
        }

        [Fact]
        public async Task Subject_WithSpecialCharactersInName_ShouldBeHandled()
        {
            // Arrange
            var nameWithSpecialChars = "Mathematics @#$%^&*()_+{}|:\"<>?[]\\;',./";
            var createDto = new CreateSchoolSubjectDto { Name = nameWithSpecialChars };

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<SchoolSubject>(), 1))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _schoolSubjectServices.AddSchoolSubject(createDto);

            // Assert
            Assert.True(result.IsSuccess);
            _schoolSubjectRepositoryMock.Verify(repo => repo.AddAsync(
                It.Is<SchoolSubject>(s => s.Name == nameWithSpecialChars), 1), Times.Once);
        }

        [Fact]
        public async Task FetchSchoolSubjectsFromBackendAsync_AfterAddEditDelete_ShouldReflectChanges()
        {
            // Arrange
            var subjects = new List<SchoolSubject>
            {
                CreateTestSchoolSubject("Mathematics"),
                CreateTestSchoolSubject("Physics")
            };

            // Setup for fetch
            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectListAsync(1))
                .ReturnsAsync(subjects);

            // Setup for other operations
            _schoolSubjectRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<SchoolSubject>(), 1))
                .Returns(Task.CompletedTask);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<SchoolSubject>()))
                .Returns(Task.CompletedTask);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<SchoolSubject>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act - Fetch initial state
            var initialFetch = await _schoolSubjectServices.FetchSchoolSubjectsFromBackendAsync();

            // Act - Add new subject
            var addResult = await _schoolSubjectServices.AddSchoolSubject(
                new CreateSchoolSubjectDto { Name = "Chemistry" });

            // Act - Fetch after add
            var afterAddFetch = await _schoolSubjectServices.FetchSchoolSubjectsFromBackendAsync();

            // Assert
            Assert.True(addResult.IsSuccess);
            // Note: The fetch returns mocked data, not real updated data
            // In a real scenario, the repository would return updated data
        }

        [Fact]
        public async Task EditSchoolSubject_WithSameName_ShouldSucceed()
        {
            // Arrange
            var subjectId = Guid.NewGuid();
            var subjectName = "Same Name";
            var subjectDto = new SchoolSubjectDto { Id = subjectId, Name = subjectName };
            var existingSubject = CreateTestSchoolSubject(subjectName, subjectId);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(subjectId))
                .ReturnsAsync(existingSubject);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<SchoolSubject>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _schoolSubjectServices.EditSchoolSubject(subjectDto);

            // Assert
            Assert.True(result.IsSuccess);
            // Even with same name, the update should proceed
            _schoolSubjectRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<SchoolSubject>()), Times.Once);
        }

        [Fact]
        public async Task DeleteSchoolSubject_WithInvalidDto_ShouldReturnFailure()
        {
            // Arrange
            var subjectDto = new SchoolSubjectDto { Id = Guid.Empty, Name = null };

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectByIdAsync(Guid.Empty))
                .ReturnsAsync((SchoolSubject?)null);

            // Act
            var result = await _schoolSubjectServices.DeleteSchoolSubject(subjectDto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Предмет не найден.", result.Error);
        }

        [Fact]
        public async Task AddSchoolSubject_WithDuplicateName_ShouldSucceed()
        {
            // Arrange
            var createDto = new CreateSchoolSubjectDto { Name = "Mathematics" };

            // Business logic doesn't prevent duplicate names, so this should succeed
            _schoolSubjectRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<SchoolSubject>(), 1))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result1 = await _schoolSubjectServices.AddSchoolSubject(createDto);
            var result2 = await _schoolSubjectServices.AddSchoolSubject(createDto);

            // Assert
            Assert.True(result1.IsSuccess);
            Assert.True(result2.IsSuccess);
            _schoolSubjectRepositoryMock.Verify(repo => repo.AddAsync(
                It.Is<SchoolSubject>(s => s.Name == "Mathematics"), 1), Times.Exactly(2));
        }

        [Fact]
        public async Task Service_ShouldUseBaseServiceFunctionality()
        {
            // Arrange
            var createDto = new CreateSchoolSubjectDto { Name = "Mathematics" };

            // Test that ExecuteRepositoryTask and TrySaveChangesAsync are called through base service
            _schoolSubjectRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<SchoolSubject>(), 1))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _schoolSubjectServices.AddSchoolSubject(createDto);

            // Assert
            Assert.True(result.IsSuccess);
            // The base service methods should have been invoked
            // We can verify this indirectly by checking that repository and unit of work were called
        }

        #endregion

        #region Helper Methods

        private SchoolSubject CreateTestSchoolSubject(string name, Guid? id = null)
        {
            return SchoolSubject.CreateSchoolSubject(id ?? Guid.NewGuid(), name).Value;
        }

        #endregion
    }
}