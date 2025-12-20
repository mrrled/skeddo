// using Application;
// using Application.DtoModels;
// using Application.Services;
// using Domain.Models;
// using Domain.IRepositories;
// using Moq;
// using Xunit;
//
// namespace Tests.ServicesTests
// {
//     public class StudyGroupServicesTests
//     {
//         private readonly Mock<IStudyGroupRepository> _studyGroupRepositoryMock;
//         private readonly Mock<IUnitOfWork> _unitOfWorkMock;
//         private readonly StudyGroupServices _service;
//         
//         public StudyGroupServicesTests()
//         {
//             _studyGroupRepositoryMock = new Mock<IStudyGroupRepository>();
//             _unitOfWorkMock = new Mock<IUnitOfWork>();
//             _service = new StudyGroupServices(_studyGroupRepositoryMock.Object, _unitOfWorkMock.Object);
//         }
//
//         [Fact]
//         public async Task FetchStudyGroupsFromBackendAsync_ShouldReturnListOfStudyGroupDtos()
//         {
//             // Arrange
//             var studyGroups = new List<StudyGroup>
//             {
//                 StudyGroup.CreateStudyGroup(Guid.NewGuid(), "Group 1"),
//                 StudyGroup.CreateStudyGroup(Guid.NewGuid(), "Group 2")
//             };
//
//             _studyGroupRepositoryMock
//                 .Setup(repo => repo.GetStudyGroupListAsync(1))
//                 .ReturnsAsync(studyGroups);
//
//             // Act
//             var result = await _service.FetchStudyGroupsFromBackendAsync();
//
//             // Assert
//             Assert.NotNull(result);
//             Assert.Equal(2, result.Count);
//             Assert.Equal("Group 1", result[0].Name);
//             Assert.Equal("Group 2", result[1].Name);
//             _studyGroupRepositoryMock.Verify(repo => repo.GetStudyGroupListAsync(1), Times.Once);
//         }
//
//         [Fact]
//         public async Task FetchStudyGroupsFromBackendAsync_WhenNoGroups_ShouldReturnEmptyList()
//         {
//             // Arrange
//             _studyGroupRepositoryMock
//                 .Setup(repo => repo.GetStudyGroupListAsync(1))
//                 .ReturnsAsync(new List<StudyGroup>());
//
//             // Act
//             var result = await _service.FetchStudyGroupsFromBackendAsync();
//
//             // Assert
//             Assert.NotNull(result);
//             Assert.Empty(result);
//         }
//
//         [Fact]
//         public async Task AddStudyGroup_ShouldAddStudyGroupAndSaveChanges()
//         {
//             // Arrange
//             var studyGroupDto = new CreateStudyGroupDto { Name = "New Group" };
//             
//             _studyGroupRepositoryMock
//                 .Setup(repo => repo.AddAsync(It.IsAny<StudyGroup>(), 1))
//                 .Returns(Task.CompletedTask);
//                 
//             _unitOfWorkMock
//                 .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
//                 .ReturnsAsync(1);
//
//             // Act
//             await _service.AddStudyGroup(studyGroupDto);
//
//             // Assert
//             _studyGroupRepositoryMock.Verify(repo => repo.AddAsync(
//                 It.Is<StudyGroup>(sg => sg.Name == "New Group"), 
//                 1), Times.Once);
//             _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//         }
//
//         [Fact]
//         public async Task AddStudyGroup_WithNullName_ShouldThrowArgumentNullException()
//         {
//             // Arrange
//             var studyGroupDto = new CreateStudyGroupDto { Name = null! };
//
//             // Act & Assert
//             await Assert.ThrowsAsync<ArgumentNullException>(() => 
//                 _service.AddStudyGroup(studyGroupDto));
//         }
//
//         [Fact]
//         public async Task EditStudyGroup_WhenGroupExistsAndNameChanged_ShouldUpdateAndSaveChanges()
//         {
//             // Arrange
//             var studyGroupId = Guid.NewGuid();
//             var studyGroupDto = new StudyGroupDto { Id = studyGroupId, Name = "Updated Name" };
//             var existingStudyGroup = StudyGroup.CreateStudyGroup(studyGroupId, "Original Name");
//             
//             _studyGroupRepositoryMock
//                 .Setup(repo => repo.GetStudyGroupByIdAsync(studyGroupId))
//                 .ReturnsAsync(existingStudyGroup);
//                 
//             _unitOfWorkMock
//                 .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
//                 .ReturnsAsync(1);
//
//             // Act
//             await _service.EditStudyGroup(studyGroupDto);
//
//             // Assert
//             Assert.Equal("Updated Name", existingStudyGroup.Name);
//             _studyGroupRepositoryMock.Verify(repo => repo.UpdateAsync(existingStudyGroup), Times.Once);
//             _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//         }
//
//         [Fact]
//         public async Task EditStudyGroup_WhenGroupExistsAndNameUnchanged_ShouldStillSaveChanges()
//         {
//             // Arrange
//             var studyGroupId = Guid.NewGuid();
//             var studyGroupDto = new StudyGroupDto { Id = studyGroupId, Name = "Same Name" };
//             var existingStudyGroup = StudyGroup.CreateStudyGroup(studyGroupId, "Same Name");
//             
//             _studyGroupRepositoryMock
//                 .Setup(repo => repo.GetStudyGroupByIdAsync(studyGroupId))
//                 .ReturnsAsync(existingStudyGroup);
//                 
//             _unitOfWorkMock
//                 .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
//                 .ReturnsAsync(1);
//
//             // Act
//             await _service.EditStudyGroup(studyGroupDto);
//
//             // Assert
//             _studyGroupRepositoryMock.Verify(repo => repo.UpdateAsync(existingStudyGroup), Times.Once);
//             _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//         }
//
//         [Fact]
//         public async Task EditStudyGroup_WhenGroupNotFound_ShouldThrowArgumentException()
//         {
//             // Arrange
//             var studyGroupId = Guid.NewGuid();
//             var studyGroupDto = new StudyGroupDto { Id = studyGroupId, Name = "Updated Name" };
//             
//             _studyGroupRepositoryMock
//                 .Setup(repo => repo.GetStudyGroupByIdAsync(studyGroupId))
//                 .ThrowsAsync(new InvalidOperationException());
//
//             // Act & Assert
//             await Assert.ThrowsAsync<ArgumentException>(() => 
//                 _service.EditStudyGroup(studyGroupDto));
//                 
//             _studyGroupRepositoryMock.Verify(repo => repo.GetStudyGroupByIdAsync(studyGroupId), Times.Once);
//             _studyGroupRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<StudyGroup>()), Times.Never);
//             _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
//         }
//
//         [Fact]
//         public async Task EditStudyGroup_WithNullName_ShouldThrowArgumentNullException()
//         {
//             // Arrange
//             var studyGroupId = Guid.NewGuid();
//             var studyGroupDto = new StudyGroupDto { Id = studyGroupId, Name = null! };
//             var existingStudyGroup = StudyGroup.CreateStudyGroup(studyGroupId, "Original Name");
//             
//             _studyGroupRepositoryMock
//                 .Setup(repo => repo.GetStudyGroupByIdAsync(studyGroupId))
//                 .ReturnsAsync(existingStudyGroup);
//
//             // Act & Assert
//             await Assert.ThrowsAsync<ArgumentNullException>(() => 
//                 _service.EditStudyGroup(studyGroupDto));
//                 
//             _studyGroupRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<StudyGroup>()), Times.Never);
//             _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
//         }
//
//         [Fact]
//         public async Task DeleteStudyGroup_WhenGroupExists_ShouldDeleteAndSaveChanges()
//         {
//             // Arrange
//             var studyGroupId = Guid.NewGuid();
//             var studyGroupDto = new StudyGroupDto { Id = studyGroupId, Name = "Group to Delete" };
//             var existingStudyGroup = StudyGroup.CreateStudyGroup(studyGroupId, "Group to Delete");
//             
//             _studyGroupRepositoryMock
//                 .Setup(repo => repo.GetStudyGroupByIdAsync(studyGroupId))
//                 .ReturnsAsync(existingStudyGroup);
//                 
//             _studyGroupRepositoryMock
//                 .Setup(repo => repo.Delete(existingStudyGroup))
//                 .Returns(Task.CompletedTask);
//                 
//             _unitOfWorkMock
//                 .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
//                 .ReturnsAsync(1);
//
//             // Act
//             await _service.DeleteStudyGroup(studyGroupDto);
//
//             // Assert
//             _studyGroupRepositoryMock.Verify(repo => repo.Delete(existingStudyGroup), Times.Once);
//             _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//         }
//
//         [Fact]
//         public async Task DeleteStudyGroup_WhenGroupNotFound_ShouldThrowArgumentException()
//         {
//             // Arrange
//             var studyGroupId = Guid.NewGuid();
//             var studyGroupDto = new StudyGroupDto { Id = studyGroupId, Name = "Non-existent Group" };
//             
//             _studyGroupRepositoryMock
//                 .Setup(repo => repo.GetStudyGroupByIdAsync(studyGroupId))
//                 .ThrowsAsync(new InvalidOperationException());
//
//             // Act & Assert
//             await Assert.ThrowsAsync<ArgumentException>(() => 
//                 _service.DeleteStudyGroup(studyGroupDto));
//                 
//             _studyGroupRepositoryMock.Verify(repo => repo.GetStudyGroupByIdAsync(studyGroupId), Times.Once);
//             _studyGroupRepositoryMock.Verify(repo => repo.Delete(It.IsAny<StudyGroup>()), Times.Never);
//             _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
//         }
//
//         [Fact]
//         public async Task SaveChangesAsync_ShouldBeCalledWithCorrectCancellationToken()
//         {
//             // Arrange
//             var studyGroupId = Guid.NewGuid();
//             var studyGroupDto = new StudyGroupDto { Id = studyGroupId, Name = "Test Group" };
//             var existingStudyGroup = StudyGroup.CreateStudyGroup(studyGroupId, "Test Group");
//             var cancellationToken = new CancellationToken(true);
//             
//             _studyGroupRepositoryMock
//                 .Setup(repo => repo.GetStudyGroupByIdAsync(studyGroupId))
//                 .ReturnsAsync(existingStudyGroup);
//
//             // Act
//             await _service.EditStudyGroup(studyGroupDto);
//
//             // Assert
//             _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(cancellationToken), Times.Once);
//         }
//     }
// }