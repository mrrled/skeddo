// using Moq;
// using Xunit;
// using Application.Services;
// using Application.DtoModels;
// using Domain.Models;
// using Domain.IRepositories;
// using Application;
//
// namespace Tests.ServicesTests;
//
// public class LessonNumberServicesTests
// {
//     private readonly Mock<ILessonNumberRepository> _mockLessonNumberRepository;
//     private readonly Mock<IUnitOfWork> _mockUnitOfWork;
//     private readonly LessonNumberServices _lessonNumberServices;
//
//     public LessonNumberServicesTests()
//     {
//         _mockLessonNumberRepository = new Mock<ILessonNumberRepository>();
//         _mockUnitOfWork = new Mock<IUnitOfWork>();
//         _lessonNumberServices = new LessonNumberServices(
//             _mockLessonNumberRepository.Object, 
//             _mockUnitOfWork.Object
//         );
//     }
//
//     [Fact]
//     public async Task GetLessonNumbersByScheduleId_WhenScheduleExists_ReturnsLessonNumberDtos()
//     {
//         // Arrange
//         var scheduleId = 1;
//         var lessonNumbers = new List<LessonNumber>
//         {
//             Schedule.CreateLessonNumber(1, new TimeSpan(8, 0, 0).ToString()),
//             Schedule.CreateLessonNumber(2, new TimeSpan(9, 0, 0).ToString())
//         };
//
//         _mockLessonNumberRepository
//             .Setup(repo => repo.GetLessonNumbersByScheduleIdAsync(scheduleId))
//             .ReturnsAsync(lessonNumbers);
//
//         // Act
//         var result = await _lessonNumberServices.GetLessonNumbersByScheduleId(scheduleId);
//
//         // Assert
//         Assert.NotNull(result);
//         Assert.Equal(2, result.Count);
//         Assert.Equal(1, result[0].Number);
//         Assert.Equal(new TimeSpan(8, 0, 0).ToString(), result[0].Time);
//         Assert.Equal(2, result[1].Number);
//         Assert.Equal(new TimeSpan(9, 0, 0).ToString(), result[1].Time);
//         
//         _mockLessonNumberRepository.Verify(repo => 
//             repo.GetLessonNumbersByScheduleIdAsync(scheduleId), Times.Once);
//     }
//
//     [Fact]
//     public async Task AddLessonNumber_ValidData_CallsRepositoryAndSavesChanges()
//     {
//         // Arrange
//         var scheduleId = 1;
//         var lessonNumberDto = new LessonNumberDto 
//         { 
//             Number = 1, 
//             Time = new TimeSpan(8, 0, 0).ToString()
//         };
//
//         _mockLessonNumberRepository
//             .Setup(repo => repo.AddAsync(It.IsAny<LessonNumber>(), scheduleId))
//             .Returns(Task.CompletedTask);
//         
//         _mockUnitOfWork
//             .Setup(uow => uow.SaveChangesAsync(default))
//             .ReturnsAsync(1);
//
//         // Act
//         await _lessonNumberServices.AddLessonNumber(lessonNumberDto, scheduleId);
//
//         // Assert
//         _mockLessonNumberRepository.Verify(repo => 
//             repo.AddAsync(It.Is<LessonNumber>(ln => 
//                 ln.Number == lessonNumberDto.Number && 
//                 ln.Time == lessonNumberDto.Time), 
//             scheduleId), 
//             Times.Once);
//         
//         _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
//     }
//
//     [Fact]
//     public async Task EditLessonNumber_ValidData_CallsRepositoryAndSavesChanges()
//     {
//         // Arrange
//         var scheduleId = 1;
//         var oldLessonNumberDto = new LessonNumberDto 
//         { 
//             Number = 1, 
//             Time = new TimeSpan(8, 0, 0).ToString() 
//         };
//         var newLessonNumberDto = new LessonNumberDto 
//         { 
//             Number = 1, 
//             Time = new TimeSpan(8, 30, 0).ToString() 
//         };
//
//         _mockLessonNumberRepository
//             .Setup(repo => repo.UpdateAsync(
//                 It.IsAny<LessonNumber>(), 
//                 It.IsAny<LessonNumber>(), 
//                 scheduleId))
//             .Returns(Task.CompletedTask);
//         
//         _mockUnitOfWork
//             .Setup(uow => uow.SaveChangesAsync(default))
//             .ReturnsAsync(1);
//
//         // Act
//         await _lessonNumberServices.EditLessonNumber(
//             oldLessonNumberDto, 
//             newLessonNumberDto, 
//             scheduleId);
//
//         // Assert
//         _mockLessonNumberRepository.Verify(repo => 
//             repo.UpdateAsync(
//                 It.Is<LessonNumber>(oldLn => 
//                     oldLn.Number == oldLessonNumberDto.Number && 
//                     oldLn.Time == oldLessonNumberDto.Time),
//                 It.Is<LessonNumber>(newLn => 
//                     newLn.Number == newLessonNumberDto.Number && 
//                     newLn.Time == newLessonNumberDto.Time),
//                 scheduleId), 
//             Times.Once);
//         
//         _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
//     }
//
//     [Fact]
//     public async Task DeleteLessonNumber_ValidData_CallsRepositoryAndSavesChanges()
//     {
//         // Arrange
//         var scheduleId = 1;
//         var lessonNumberDto = new LessonNumberDto 
//         { 
//             Number = 1, 
//             Time = new TimeSpan(8, 0, 0).ToString() 
//         };
//
//         _mockLessonNumberRepository
//             .Setup(repo => repo.Delete(It.IsAny<LessonNumber>(), scheduleId))
//             .Returns(Task.CompletedTask);
//         
//         _mockUnitOfWork
//             .Setup(uow => uow.SaveChangesAsync(default))
//             .ReturnsAsync(1);
//
//         // Act
//         await _lessonNumberServices.DeleteLessonNumber(lessonNumberDto, scheduleId);
//
//         // Assert
//         _mockLessonNumberRepository.Verify(repo => 
//             repo.Delete(It.Is<LessonNumber>(ln => 
//                 ln.Number == lessonNumberDto.Number && 
//                 ln.Time == lessonNumberDto.Time), 
//             scheduleId), 
//             Times.Once);
//         
//         _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
//     }
//
//     [Fact]
//     public async Task GetLessonNumbersByScheduleId_RepositoryThrowsException_PropagatesException()
//     {
//         // Arrange
//         var scheduleId = 1;
//         var expectedException = new NullReferenceException("Schedule not found");
//
//         _mockLessonNumberRepository
//             .Setup(repo => repo.GetLessonNumbersByScheduleIdAsync(scheduleId))
//             .ThrowsAsync(expectedException);
//
//         // Act & Assert
//         var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
//             _lessonNumberServices.GetLessonNumbersByScheduleId(scheduleId));
//         
//         Assert.Equal(expectedException.Message, exception.Message);
//         _mockLessonNumberRepository.Verify(repo => 
//             repo.GetLessonNumbersByScheduleIdAsync(scheduleId), Times.Once);
//     }
//
//     [Fact]
//     public async Task AddLessonNumber_RepositoryThrowsException_DoesNotSaveChanges()
//     {
//         // Arrange
//         var scheduleId = 1;
//         var lessonNumberDto = new LessonNumberDto 
//         { 
//             Number = 1, 
//             Time = new TimeSpan(8, 0, 0).ToString() 
//         };
//
//         _mockLessonNumberRepository
//             .Setup(repo => repo.AddAsync(It.IsAny<LessonNumber>(), scheduleId))
//             .ThrowsAsync(new NullReferenceException("Schedule not found"));
//
//         // Act & Assert
//         await Assert.ThrowsAsync<NullReferenceException>(() =>
//             _lessonNumberServices.AddLessonNumber(lessonNumberDto, scheduleId));
//         
//         _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Never);
//     }
// }