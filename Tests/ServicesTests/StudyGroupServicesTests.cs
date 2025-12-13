// using Application.DtoModels;
// using Application.Services;
// using Domain.IRepositories;
// using Domain.Models;
// using Moq;
// using Xunit;
// using Application;
// // ReSharper disable PreferConcreteValueOverDefault
//
// namespace Tests.ServicesTests;
//
// public class StudyGroupServicesTests
// {
//     private readonly Mock<IStudyGroupRepository> _studyGroupRepositoryMock;
//     private readonly Mock<IUnitOfWork> _unitOfWorkMock;
//     private readonly StudyGroupServices _studyGroupServices;
//
//     public StudyGroupServicesTests()
//     {
//         _studyGroupRepositoryMock = new Mock<IStudyGroupRepository>();
//         _unitOfWorkMock = new Mock<IUnitOfWork>();
//         _studyGroupServices = new StudyGroupServices(
//             _studyGroupRepositoryMock.Object, 
//             _unitOfWorkMock.Object
//         );
//     }
//
//     [Fact]
//     public async Task FetchStudyGroupsFromBackendAsync_ReturnsStudyGroupDtoList()
//     {
//         // Arrange
//         var studyGroups = new List<StudyGroup>
//         {
//             Schedule.CreateStudyGroup("Group1"),
//             Schedule.CreateStudyGroup("Group2")
//         };
//
//         _studyGroupRepositoryMock
//             .Setup(repo => repo.GetStudyGroupListAsync())
//             .ReturnsAsync(studyGroups);
//
//         // Act
//         var result = await _studyGroupServices.FetchStudyGroupsFromBackendAsync();
//
//         // Assert
//         Assert.NotNull(result);
//         Assert.IsType<List<StudyGroupDto>>(result);
//         Assert.Equal(studyGroups.Count, result.Count);
//         _studyGroupRepositoryMock.Verify(repo => repo.GetStudyGroupListAsync(), Times.Once);
//     }
//
//     [Fact]
//     public async Task AddStudyGroup_ValidDto_CallsRepositoryAndSavesChanges()
//     {
//         // Arrange
//         var studyGroupDto = new StudyGroupDto { Name = "TestGroup" };
//         Schedule.CreateStudyGroup(studyGroupDto.Name);
//
//         _studyGroupRepositoryMock
//             .Setup(repo => repo.AddAsync(It.IsAny<StudyGroup>()))
//             .Returns(Task.CompletedTask);
//
//         _unitOfWorkMock
//             .Setup(uow => uow.SaveChangesAsync(default))
//             .ReturnsAsync(1);
//
//         // Act
//         await _studyGroupServices.AddStudyGroup(studyGroupDto);
//
//         // Assert
//         _studyGroupRepositoryMock.Verify(
//             repo => repo.AddAsync(It.Is<StudyGroup>(sg => sg.Name == studyGroupDto.Name)), 
//             Times.Once
//         );
//         _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
//     }
//
//     [Fact]
//     public async Task EditStudyGroup_ValidDtos_CallsRepositoryAndSavesChanges()
//     {
//         // Arrange
//         var oldStudyGroupDto = new StudyGroupDto { Name = "OldGroup" };
//         var newStudyGroupDto = new StudyGroupDto { Name = "NewGroup" };
//
//         _studyGroupRepositoryMock
//             .Setup(repo => repo.UpdateAsync(It.IsAny<StudyGroup>(), It.IsAny<StudyGroup>()))
//             .Returns(Task.CompletedTask);
//
//         _unitOfWorkMock
//             .Setup(uow => uow.SaveChangesAsync(default))
//             .ReturnsAsync(1);
//
//         // Act
//         await _studyGroupServices.EditStudyGroup(oldStudyGroupDto, newStudyGroupDto);
//
//         // Assert
//         _studyGroupRepositoryMock.Verify(
//             repo => repo.UpdateAsync(
//                 It.Is<StudyGroup>(old => old.Name == oldStudyGroupDto.Name),
//                 It.Is<StudyGroup>(newSg => newSg.Name == newStudyGroupDto.Name)
//             ), 
//             Times.Once
//         );
//         _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
//     }
//
//     [Fact]
//     public async Task DeleteStudyGroup_ValidDto_CallsRepositoryAndSavesChanges()
//     {
//         // Arrange
//         var studyGroupDto = new StudyGroupDto { Name = "GroupToDelete" };
//
//         _studyGroupRepositoryMock
//             .Setup(repo => repo.Delete(It.IsAny<StudyGroup>()))
//             .Returns(Task.CompletedTask);
//
//         _unitOfWorkMock
//             .Setup(uow => uow.SaveChangesAsync(default))
//             .ReturnsAsync(1);
//
//         // Act
//         await _studyGroupServices.DeleteStudyGroup(studyGroupDto);
//
//         // Assert
//         _studyGroupRepositoryMock.Verify(
//             repo => repo.Delete(It.Is<StudyGroup>(sg => sg.Name == studyGroupDto.Name)), 
//             Times.Once
//         );
//         _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
//     }
//
//     [Fact]
//     public async Task FetchStudyGroupsFromBackendAsync_RepositoryThrowsException_ThrowsException()
//     {
//         // Arrange
//         _studyGroupRepositoryMock
//             .Setup(repo => repo.GetStudyGroupListAsync())
//             .ThrowsAsync(new Exception("Repository error"));
//
//         // Act & Assert
//         await Assert.ThrowsAsync<Exception>(() => 
//             _studyGroupServices.FetchStudyGroupsFromBackendAsync()
//         );
//     }
//
//     [Fact]
//     public async Task AddStudyGroup_RepositoryThrowsException_DoesNotSaveChanges()
//     {
//         // Arrange
//         var studyGroupDto = new StudyGroupDto { Name = "TestGroup" };
//
//         _studyGroupRepositoryMock
//             .Setup(repo => repo.AddAsync(It.IsAny<StudyGroup>()))
//             .ThrowsAsync(new Exception("Add failed"));
//
//         _unitOfWorkMock
//             .Setup(uow => uow.SaveChangesAsync(default))
//             .ReturnsAsync(1);
//
//         // Act & Assert
//         await Assert.ThrowsAsync<Exception>(() => 
//             _studyGroupServices.AddStudyGroup(studyGroupDto)
//         );
//         
//         _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Never);
//     }
//
//     [Fact]
//     public async Task EditStudyGroup_RepositoryThrowsException_DoesNotSaveChanges()
//     {
//         // Arrange
//         var oldStudyGroupDto = new StudyGroupDto { Name = "OldGroup" };
//         var newStudyGroupDto = new StudyGroupDto { Name = "NewGroup" };
//
//         _studyGroupRepositoryMock
//             .Setup(repo => repo.UpdateAsync(It.IsAny<StudyGroup>(), It.IsAny<StudyGroup>()))
//             .ThrowsAsync(new Exception("Update failed"));
//
//         _unitOfWorkMock
//             .Setup(uow => uow.SaveChangesAsync(default))
//             .ReturnsAsync(1);
//
//         // Act & Assert
//         await Assert.ThrowsAsync<Exception>(() => 
//             _studyGroupServices.EditStudyGroup(oldStudyGroupDto, newStudyGroupDto)
//         );
//         
//         _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Never);
//     }
//
//     [Fact]
//     public async Task DeleteStudyGroup_RepositoryThrowsException_DoesNotSaveChanges()
//     {
//         // Arrange
//         var studyGroupDto = new StudyGroupDto { Name = "GroupToDelete" };
//
//         _studyGroupRepositoryMock
//             .Setup(repo => repo.Delete(It.IsAny<StudyGroup>()))
//             .ThrowsAsync(new Exception("Delete failed"));
//
//         _unitOfWorkMock
//             .Setup(uow => uow.SaveChangesAsync(default))
//             .ReturnsAsync(1);
//
//         // Act & Assert
//         await Assert.ThrowsAsync<Exception>(() => 
//             _studyGroupServices.DeleteStudyGroup(studyGroupDto)
//         );
//         
//         _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Never);
//     }
// }