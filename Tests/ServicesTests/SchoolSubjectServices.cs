// using Application.DtoModels;
// using Application.IServices;
// using Application.Services;
// using Domain.Models;
// using Domain.IRepositories;
// using Moq;
// using Xunit;
// using Application;
// // ReSharper disable PreferConcreteValueOverDefault
//
// namespace Tests.ServicesTests
// {
//     public class SchoolSubjectServicesTests
//     {
//         private readonly Mock<ISchoolSubjectRepository> _mockSchoolSubjectRepository;
//         private readonly Mock<IUnitOfWork> _mockUnitOfWork;
//         private readonly ISchoolSubjectServices _schoolSubjectServices;
//
//         public SchoolSubjectServicesTests()
//         {
//             _mockSchoolSubjectRepository = new Mock<ISchoolSubjectRepository>();
//             _mockUnitOfWork = new Mock<IUnitOfWork>();
//             _schoolSubjectServices = new SchoolSubjectServices(
//                 _mockSchoolSubjectRepository.Object,
//                 _mockUnitOfWork.Object
//             );
//         }
//
//         [Fact]
//         public async Task FetchSchoolSubjectsFromBackendAsync_ShouldReturnListOfSchoolSubjectDto()
//         {
//             // Arrange
//             var schoolSubjects = new List<SchoolSubject>
//             {
//                 Schedule.CreateSchoolSubject("Mathematics"),
//                 Schedule.CreateSchoolSubject("Physics"),
//                 Schedule.CreateSchoolSubject("Chemistry")
//             };
//
//             _mockSchoolSubjectRepository
//                 .Setup(repo => repo.GetSchoolSubjectListAsync())
//                 .ReturnsAsync(schoolSubjects);
//
//             // Act
//             var result = await _schoolSubjectServices.FetchSchoolSubjectsFromBackendAsync();
//
//             // Assert
//             Assert.NotNull(result);
//             Assert.Equal(schoolSubjects.Count, result.Count);
//         }
//
//         [Fact]
//         public async Task AddSchoolSubject_ShouldCallRepositoryAndSaveChanges()
//         {
//             // Arrange
//             var schoolSubjectDto = new SchoolSubjectDto { Name = "Biology" };
//             var schoolSubject = Schedule.CreateSchoolSubject("Biology");
//
//             _mockSchoolSubjectRepository
//                 .Setup(repo => repo.AddAsync(It.IsAny<SchoolSubject>()))
//                 .Returns(Task.CompletedTask)
//                 .Verifiable();
//
//             _mockUnitOfWork
//                 .Setup(uow => uow.SaveChangesAsync(default))
//                 .ReturnsAsync(1)
//                 .Verifiable();
//
//             // Act
//             await _schoolSubjectServices.AddSchoolSubject(schoolSubjectDto);
//
//             // Assert
//             _mockSchoolSubjectRepository.Verify(
//                 repo => repo.AddAsync(It.Is<SchoolSubject>(s => s.Name == schoolSubject.Name)),
//                 Times.Once
//             );
//             _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
//         }
//
//         [Fact]
//         public async Task EditSchoolSubject_ShouldCallRepositoryAndSaveChanges()
//         {
//             // Arrange
//             var oldSubjectDto = new SchoolSubjectDto { Name = "Math" };
//             var newSubjectDto = new SchoolSubjectDto { Name = "Advanced Mathematics" };
//             
//             var oldSchoolSubject = Schedule.CreateSchoolSubject("Math");
//             var newSchoolSubject = Schedule.CreateSchoolSubject("Advanced Mathematics");
//
//             _mockSchoolSubjectRepository
//                 .Setup(repo => repo.UpdateAsync(
//                     It.Is<SchoolSubject>(s => s.Name == oldSchoolSubject.Name),
//                     It.Is<SchoolSubject>(s => s.Name == newSchoolSubject.Name)
//                 ))
//                 .Returns(Task.CompletedTask)
//                 .Verifiable();
//
//             _mockUnitOfWork
//                 .Setup(uow => uow.SaveChangesAsync(default))
//                 .ReturnsAsync(1)
//                 .Verifiable();
//
//             // Act
//             await _schoolSubjectServices.EditSchoolSubject(oldSubjectDto, newSubjectDto);
//
//             // Assert
//             _mockSchoolSubjectRepository.Verify(
//                 repo => repo.UpdateAsync(
//                     It.Is<SchoolSubject>(s => s.Name == oldSchoolSubject.Name),
//                     It.Is<SchoolSubject>(s => s.Name == newSchoolSubject.Name)
//                 ),
//                 Times.Once
//             );
//             _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
//         }
//
//         [Fact]
//         public async Task DeleteSchoolSubject_ShouldCallRepositoryAndSaveChanges()
//         {
//             // Arrange
//             var schoolSubjectDto = new SchoolSubjectDto { Name = "History" };
//             var schoolSubject = Schedule.CreateSchoolSubject("History");
//
//             _mockSchoolSubjectRepository
//                 .Setup(repo => repo.Delete(It.IsAny<SchoolSubject>()))
//                 .Returns(Task.CompletedTask)
//                 .Verifiable();
//
//             _mockUnitOfWork
//                 .Setup(uow => uow.SaveChangesAsync(default))
//                 .ReturnsAsync(1)
//                 .Verifiable();
//
//             // Act
//             await _schoolSubjectServices.DeleteSchoolSubject(schoolSubjectDto);
//
//             // Assert
//             _mockSchoolSubjectRepository.Verify(
//                 repo => repo.Delete(It.Is<SchoolSubject>(s => s.Name == schoolSubject.Name)),
//                 Times.Once
//             );
//             _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
//         }
//
//         [Fact]
//         public async Task FetchSchoolSubjectsFromBackendAsync_ShouldHandleEmptyList()
//         {
//             // Arrange
//             var emptyList = new List<SchoolSubject>();
//             
//             _mockSchoolSubjectRepository
//                 .Setup(repo => repo.GetSchoolSubjectListAsync())
//                 .ReturnsAsync(emptyList);
//
//             // Act
//             var result = await _schoolSubjectServices.FetchSchoolSubjectsFromBackendAsync();
//
//             // Assert
//             Assert.NotNull(result);
//             Assert.Empty(result);
//         }
//
//         [Fact]
//         public async Task AddSchoolSubject_ShouldThrowIfRepositoryThrows()
//         {
//             // Arrange
//             var schoolSubjectDto = new SchoolSubjectDto { Name = "Test" };
//             
//             _mockSchoolSubjectRepository
//                 .Setup(repo => repo.AddAsync(It.IsAny<SchoolSubject>()))
//                 .ThrowsAsync(new Exception("Database error"));
//
//             // Act & Assert
//             await Assert.ThrowsAsync<Exception>(
//                 () => _schoolSubjectServices.AddSchoolSubject(schoolSubjectDto)
//             );
//             
//             _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Never);
//         }
//     }
// }