using Application.DtoModels;
using Application.Services;
using Domain.Models;
using Domain.IRepositories;
using Moq;
using Xunit;
using Application;
// ReSharper disable PreferConcreteValueOverDefault
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

namespace Tests.ServicesTests
{
    public class SchoolSubjectServicesTests
    {
        private readonly Mock<ISchoolSubjectRepository> _mockRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly SchoolSubjectServices _services;

        public SchoolSubjectServicesTests()
        {
            _mockRepository = new Mock<ISchoolSubjectRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _services = new SchoolSubjectServices(_mockRepository.Object, _mockUnitOfWork.Object);
        }

        #region FetchSchoolSubjectsFromBackendAsync Tests

        [Fact]
        public async Task FetchSchoolSubjectsFromBackendAsync_ShouldReturnSchoolSubjectDtos()
        {
            // Arrange
            var scheduleGroupId = 1;
            var schoolSubjects = new List<SchoolSubject>
            {
                SchoolSubject.CreateSchoolSubject(1, "Mathematics"),
                SchoolSubject.CreateSchoolSubject(2, "Physics")
            };

            _mockRepository.Setup(r => r.GetSchoolSubjectListAsync(scheduleGroupId))
                .ReturnsAsync(schoolSubjects);

            // Act
            var result = await _services.FetchSchoolSubjectsFromBackendAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Mathematics", result[0].Name);
            Assert.Equal("Physics", result[1].Name);
            _mockRepository.Verify(r => r.GetSchoolSubjectListAsync(scheduleGroupId), Times.Once);
        }

        [Fact]
        public async Task FetchSchoolSubjectsFromBackendAsync_ShouldReturnEmptyList_WhenNoSubjects()
        {
            // Arrange
            var scheduleGroupId = 1;
            var emptyList = new List<SchoolSubject>();

            _mockRepository.Setup(r => r.GetSchoolSubjectListAsync(scheduleGroupId))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _services.FetchSchoolSubjectsFromBackendAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockRepository.Verify(r => r.GetSchoolSubjectListAsync(scheduleGroupId), Times.Once);
        }

        #endregion

        #region AddSchoolSubject Tests

        [Fact]
        public async Task AddSchoolSubject_ShouldAddSubject_WhenValidDto()
        {
            // Arrange
            var schoolSubjectDto = new SchoolSubjectDto { Id = 1, Name = "Chemistry" };
            var scheduleGroupId = 1;

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<SchoolSubject>(), scheduleGroupId))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            await _services.AddSchoolSubject(schoolSubjectDto);

            // Assert
            _mockRepository.Verify(r => r.AddAsync(
                It.Is<SchoolSubject>(s => s.Id == 1 && s.Name == "Chemistry"), 
                scheduleGroupId), 
                Times.Once);
            
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task AddSchoolSubject_ShouldThrowException_WhenRepositoryThrows()
        {
            // Arrange
            var schoolSubjectDto = new SchoolSubjectDto { Id = 1, Name = "Chemistry" };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<SchoolSubject>(), 1))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default))
                .ReturnsAsync(0);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _services.AddSchoolSubject(schoolSubjectDto));
        }

        [Fact]
        public async Task AddSchoolSubject_ShouldHandleSaveChangesFailure()
        {
            // Arrange
            var schoolSubjectDto = new SchoolSubjectDto { Id = 1, Name = "Chemistry" };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<SchoolSubject>(), 1))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default))
                .ReturnsAsync(0); // No changes saved

            // Act
            await _services.AddSchoolSubject(schoolSubjectDto);

            // Assert - Should not throw even if SaveChanges returns 0
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<SchoolSubject>(), 1), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        #endregion

        #region EditSchoolSubject Tests

        [Fact]
        public async Task EditSchoolSubject_ShouldUpdateSubject_WhenNameChanged()
        {
            // Arrange
            var schoolSubjectDto = new SchoolSubjectDto { Id = 1, Name = "Updated Mathematics" };
            var existingSubject = SchoolSubject.CreateSchoolSubject(1, "Mathematics");

            _mockRepository.Setup(r => r.GetSchoolSubjectByIdAsync(schoolSubjectDto.Id))
                .ReturnsAsync(existingSubject);

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<SchoolSubject>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            await _services.EditSchoolSubject(schoolSubjectDto);

            // Assert
            _mockRepository.Verify(r => r.GetSchoolSubjectByIdAsync(schoolSubjectDto.Id), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(
                It.Is<SchoolSubject>(s => s.Id == 1 && s.Name == "Updated Mathematics")), 
                Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task EditSchoolSubject_ShouldNotCallUpdate_WhenNameNotChanged()
        {
            // Arrange
            var schoolSubjectDto = new SchoolSubjectDto { Id = 1, Name = "Mathematics" };
            var existingSubject = SchoolSubject.CreateSchoolSubject(1, "Mathematics");

            _mockRepository.Setup(r => r.GetSchoolSubjectByIdAsync(schoolSubjectDto.Id))
                .ReturnsAsync(existingSubject);

            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            await _services.EditSchoolSubject(schoolSubjectDto);

            // Assert
            _mockRepository.Verify(r => r.GetSchoolSubjectByIdAsync(schoolSubjectDto.Id), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<SchoolSubject>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task EditSchoolSubject_ShouldThrowArgumentException_WhenSubjectNotFound()
        {
            // Arrange
            var schoolSubjectDto = new SchoolSubjectDto { Id = 999, Name = "Non-existent" };

            _mockRepository.Setup(r => r.GetSchoolSubjectByIdAsync(schoolSubjectDto.Id))
                .ReturnsAsync((SchoolSubject)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _services.EditSchoolSubject(schoolSubjectDto));
            
            Assert.Contains($"School subject with id {schoolSubjectDto.Id} not found", exception.Message);
            _mockRepository.Verify(r => r.GetSchoolSubjectByIdAsync(schoolSubjectDto.Id), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<SchoolSubject>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task EditSchoolSubject_ShouldThrowArgumentNullException_WhenSettingNullName()
        {
            // Arrange
            var schoolSubjectDto = new SchoolSubjectDto { Id = 1, Name = null };
            var existingSubject = SchoolSubject.CreateSchoolSubject(1, "Mathematics");

            _mockRepository.Setup(r => r.GetSchoolSubjectByIdAsync(schoolSubjectDto.Id))
                .ReturnsAsync(existingSubject);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _services.EditSchoolSubject(schoolSubjectDto));
        }

        #endregion

        #region DeleteSchoolSubject Tests

        [Fact]
        public async Task DeleteSchoolSubject_ShouldDeleteSubject_WhenExists()
        {
            // Arrange
            var schoolSubjectDto = new SchoolSubjectDto { Id = 1, Name = "Mathematics" };
            var existingSubject = SchoolSubject.CreateSchoolSubject(1, "Mathematics");

            _mockRepository.Setup(r => r.GetSchoolSubjectByIdAsync(schoolSubjectDto.Id))
                .ReturnsAsync(existingSubject);

            _mockRepository.Setup(r => r.Delete(It.IsAny<SchoolSubject>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            await _services.DeleteSchoolSubject(schoolSubjectDto);

            // Assert
            _mockRepository.Verify(r => r.GetSchoolSubjectByIdAsync(schoolSubjectDto.Id), Times.Once);
            _mockRepository.Verify(r => r.Delete(existingSubject), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteSchoolSubject_ShouldThrowArgumentException_WhenSubjectNotFound()
        {
            // Arrange
            var schoolSubjectDto = new SchoolSubjectDto { Id = 999, Name = "Non-existent" };

            _mockRepository.Setup(r => r.GetSchoolSubjectByIdAsync(schoolSubjectDto.Id))
                .ReturnsAsync((SchoolSubject)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _services.DeleteSchoolSubject(schoolSubjectDto));
            
            Assert.Contains($"School subject with id {schoolSubjectDto.Id} not found", exception.Message);
            _mockRepository.Verify(r => r.GetSchoolSubjectByIdAsync(schoolSubjectDto.Id), Times.Once);
            _mockRepository.Verify(r => r.Delete(It.IsAny<SchoolSubject>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task DeleteSchoolSubject_ShouldHandleRepositoryDeleteException()
        {
            // Arrange
            var schoolSubjectDto = new SchoolSubjectDto { Id = 1, Name = "Mathematics" };
            var existingSubject = SchoolSubject.CreateSchoolSubject(1, "Mathematics");

            _mockRepository.Setup(r => r.GetSchoolSubjectByIdAsync(schoolSubjectDto.Id))
                .ReturnsAsync(existingSubject);

            _mockRepository.Setup(r => r.Delete(It.IsAny<SchoolSubject>()))
                .ThrowsAsync(new InvalidOperationException("Delete failed"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _services.DeleteSchoolSubject(schoolSubjectDto));
        }

        #endregion

        #region Integration-like Tests

        [Fact]
        public async Task CompleteFlow_AddEditDelete_Success()
        {
            // Test complete flow: Add → Edit → Delete
            var subjectDto = new SchoolSubjectDto { Id = 100, Name = "Test Subject" };
            var existingSubject = SchoolSubject.CreateSchoolSubject(100, "Test Subject");
            var updatedSubjectDto = new SchoolSubjectDto { Id = 100, Name = "Updated Test Subject" };

            // 1. Add
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<SchoolSubject>(), 1))
                .Returns(Task.CompletedTask);

            await _services.AddSchoolSubject(subjectDto);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<SchoolSubject>(), 1), Times.Once);

            // 2. Edit
            _mockRepository.Setup(r => r.GetSchoolSubjectByIdAsync(100))
                .ReturnsAsync(existingSubject);

            await _services.EditSchoolSubject(updatedSubjectDto);
            _mockRepository.Verify(r => r.GetSchoolSubjectByIdAsync(100), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<SchoolSubject>()), Times.Once);

            // 3. Delete
            await _services.DeleteSchoolSubject(updatedSubjectDto);
            _mockRepository.Verify(r => r.GetSchoolSubjectByIdAsync(100), Times.Exactly(2));
            _mockRepository.Verify(r => r.Delete(It.IsAny<SchoolSubject>()), Times.Once);

            // Verify SaveChanges was called 3 times total
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Exactly(3));
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task EditSchoolSubject_ShouldHandleEmptyName()
        {
            // Arrange
            var schoolSubjectDto = new SchoolSubjectDto { Id = 1, Name = "" }; // Empty string
            var existingSubject = SchoolSubject.CreateSchoolSubject(1, "Old Name");

            _mockRepository.Setup(r => r.GetSchoolSubjectByIdAsync(schoolSubjectDto.Id))
                .ReturnsAsync(existingSubject);

            // Act
            await _services.EditSchoolSubject(schoolSubjectDto);

            // Assert - Should allow empty string (not null)
            _mockRepository.Verify(r => r.UpdateAsync(
                It.Is<SchoolSubject>(s => s.Name == "")), 
                Times.Once);
        }

        [Fact]
        public async Task AddSchoolSubject_ShouldHandleZeroId()
        {
            // Arrange
            var schoolSubjectDto = new SchoolSubjectDto { Id = 0, Name = "New Subject" };

            // Act
            await _services.AddSchoolSubject(schoolSubjectDto);

            // Assert - Should handle zero ID (likely for new entities)
            _mockRepository.Verify(r => r.AddAsync(
                It.Is<SchoolSubject>(s => s.Id == 0), 
                1), 
                Times.Once);
        }

        [Fact]
        public async Task FetchSchoolSubjects_ShouldUseHardcodedScheduleGroupId()
        {
            // Arrange
            var hardcodedId = 1;
            var schoolSubjects = new List<SchoolSubject>
            {
                SchoolSubject.CreateSchoolSubject(1, "Test")
            };

            _mockRepository.Setup(r => r.GetSchoolSubjectListAsync(hardcodedId))
                .ReturnsAsync(schoolSubjects);

            // Act
            var result = await _services.FetchSchoolSubjectsFromBackendAsync();

            // Assert
            _mockRepository.Verify(r => r.GetSchoolSubjectListAsync(hardcodedId), Times.Once);
        }

        #endregion

        #region Exception Propagation Tests

        [Fact]
        public async Task AddSchoolSubject_ShouldPropagateArgumentNullException_FromCreateSchoolSubject()
        {
            // Arrange
            var schoolSubjectDto = new SchoolSubjectDto { Id = 1, Name = null };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _services.AddSchoolSubject(schoolSubjectDto));
            
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<SchoolSubject>(), It.IsAny<int>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task EditSchoolSubject_ShouldPropagateRepositoryExceptions()
        {
            // Arrange
            var schoolSubjectDto = new SchoolSubjectDto { Id = 1, Name = "Math" };

            _mockRepository.Setup(r => r.GetSchoolSubjectByIdAsync(schoolSubjectDto.Id))
                .ThrowsAsync(new InvalidOperationException("DB connection failed"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _services.EditSchoolSubject(schoolSubjectDto));
        }

        #endregion
    }
}