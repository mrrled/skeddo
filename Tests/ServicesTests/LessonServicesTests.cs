// using Moq;
// using Xunit;
// using Application.Services;
// using Application.DtoModels;
// using Domain.Models;
// using Domain.IRepositories;
// using Application;
// // ReSharper disable PreferConcreteValueOverDefault
//
// namespace Tests.ServicesTests;
//
// public class LessonServicesTests
// {
//     private readonly Mock<IScheduleRepository> _scheduleRepositoryMock;
//     private readonly Mock<ILessonRepository> _lessonRepositoryMock;
//     private readonly Mock<IUnitOfWork> _unitOfWorkMock;
//     private readonly LessonServices _lessonServices;
//
//     public LessonServicesTests()
//     {
//         _scheduleRepositoryMock = new Mock<IScheduleRepository>();
//         _lessonRepositoryMock = new Mock<ILessonRepository>();
//         _unitOfWorkMock = new Mock<IUnitOfWork>();
//         _lessonServices = new LessonServices(
//             _scheduleRepositoryMock.Object,
//             _lessonRepositoryMock.Object,
//             _unitOfWorkMock.Object
//         );
//     }
//
//     [Fact]
//     public async Task GetLessonsByScheduleId_ShouldReturnLessonsDto()
//     {
//         // Arrange
//         var scheduleId = 1;
//         var lessons = new List<Lesson>
//         {
//             CreateTestLesson(1),
//             CreateTestLesson(2)
//         };
//         
//         _lessonRepositoryMock
//             .Setup(repo => repo.GetLessonsByScheduleIdAsync(scheduleId))
//             .ReturnsAsync(lessons);
//
//         // Act
//         var result = await _lessonServices.GetLessonsByScheduleId(scheduleId);
//
//         // Assert
//         Assert.NotNull(result);
//         Assert.Equal(2, result.Count);
//         _lessonRepositoryMock.Verify(repo => repo.GetLessonsByScheduleIdAsync(scheduleId), Times.Once);
//     }
//
//     [Fact]
//     public async Task AddLesson_WithTeacher_ShouldAddLessonSuccessfully()
//     {
//         // Arrange
//         var scheduleId = 1;
//         var lessonDto = CreateTestLessonDto(1);
//         var schedule = CreateTestSchedule(scheduleId);
//
//         _scheduleRepositoryMock
//             .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
//             .ReturnsAsync(schedule);
//         
//         _lessonRepositoryMock
//             .Setup(repo => repo.AddAsync(It.IsAny<Lesson>(), scheduleId))
//             .Returns(Task.CompletedTask);
//         
//         _unitOfWorkMock
//             .Setup(uow => uow.SaveChangesAsync(default))
//             .ReturnsAsync(1);
//
//         // Act
//         await _lessonServices.AddLesson(lessonDto, scheduleId);
//
//         // Assert
//         _scheduleRepositoryMock.Verify(repo => repo.GetScheduleByIdAsync(scheduleId), Times.Once);
//         _lessonRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Lesson>(), scheduleId), Times.Once);
//         _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
//     }
//
//     [Fact]
//     public async Task AddLesson_WithoutTeacher_ShouldAddLessonSuccessfully()
//     {
//         // Arrange
//         var scheduleId = 1;
//         var lessonDto = CreateTestLessonDto(1);
//         lessonDto.Teacher = null; // No teacher
//         var schedule = CreateTestSchedule(scheduleId);
//
//         _scheduleRepositoryMock
//             .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
//             .ReturnsAsync(schedule);
//         
//         _lessonRepositoryMock
//             .Setup(repo => repo.AddAsync(It.IsAny<Lesson>(), scheduleId))
//             .Returns(Task.CompletedTask);
//         
//         _unitOfWorkMock
//             .Setup(uow => uow.SaveChangesAsync(default))
//             .ReturnsAsync(1);
//
//         // Act
//         await _lessonServices.AddLesson(lessonDto, scheduleId);
//
//         // Assert
//         _scheduleRepositoryMock.Verify(repo => repo.GetScheduleByIdAsync(scheduleId), Times.Once);
//         _lessonRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Lesson>(), scheduleId), Times.Once);
//         _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
//     }
//
//     [Fact]
//     public async Task AddLesson_ScheduleNotFound_ShouldThrowException()
//     {
//         // Arrange
//         var scheduleId = 1;
//         var lessonDto = CreateTestLessonDto(1);
//
//         _scheduleRepositoryMock
//             .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
//             .ReturnsAsync((Schedule)null);
//
//         // Act & Assert
//         await Assert.ThrowsAsync<NullReferenceException>(() => 
//             _lessonServices.AddLesson(lessonDto, scheduleId));
//     }
//
//     [Fact]
//     public async Task EditLesson_ShouldUpdateLessonSuccessfully()
//     {
//         // Arrange
//         var scheduleId = 1;
//         var lessonDto = CreateTestLessonDto(1);
//         var existingLesson = CreateTestLesson(1);
//
//         _lessonRepositoryMock
//             .Setup(repo => repo.GetLessonByIdAsync(lessonDto.Id, scheduleId))
//             .ReturnsAsync(existingLesson);
//         
//         _lessonRepositoryMock
//             .Setup(repo => repo.UpdateAsync(It.IsAny<Lesson>(), scheduleId))
//             .Returns(Task.CompletedTask);
//         
//         _unitOfWorkMock
//             .Setup(uow => uow.SaveChangesAsync(default))
//             .ReturnsAsync(1);
//
//         // Act
//         await _lessonServices.EditLesson(lessonDto, scheduleId);
//
//         // Assert
//         _lessonRepositoryMock.Verify(repo => repo.GetLessonByIdAsync(lessonDto.Id, scheduleId), Times.Once);
//         _lessonRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Lesson>(), scheduleId), Times.Once);
//         _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
//     }
//
//     [Fact]
//     public async Task EditLesson_LessonNotFound_ShouldThrowException()
//     {
//         // Arrange
//         var scheduleId = 1;
//         var lessonDto = CreateTestLessonDto(1);
//
//         _lessonRepositoryMock
//             .Setup(repo => repo.GetLessonByIdAsync(lessonDto.Id, scheduleId))
//             .ThrowsAsync(new NullReferenceException());
//
//         // Act & Assert
//         await Assert.ThrowsAsync<NullReferenceException>(() => 
//             _lessonServices.EditLesson(lessonDto, scheduleId));
//     }
//
//     [Fact]
//     public async Task DeleteLesson_ShouldDeleteLessonSuccessfully()
//     {
//         // Arrange
//         var scheduleId = 1;
//         var lessonDto = CreateTestLessonDto(1);
//         var existingLesson = CreateTestLesson(1);
//
//         _lessonRepositoryMock
//             .Setup(repo => repo.GetLessonByIdAsync(lessonDto.Id, scheduleId))
//             .ReturnsAsync(existingLesson);
//         
//         _lessonRepositoryMock
//             .Setup(repo => repo.Delete(It.IsAny<Lesson>(), scheduleId))
//             .Returns(Task.CompletedTask);
//         
//         _unitOfWorkMock
//             .Setup(uow => uow.SaveChangesAsync(default))
//             .ReturnsAsync(1);
//
//         // Act
//         await _lessonServices.DeleteLesson(lessonDto, scheduleId);
//
//         // Assert
//         _lessonRepositoryMock.Verify(repo => repo.GetLessonByIdAsync(lessonDto.Id, scheduleId), Times.Once);
//         _lessonRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Lesson>(), scheduleId), Times.Once);
//         _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
//     }
//
//     [Fact]
//     public async Task DeleteLesson_LessonNotFound_ShouldThrowException()
//     {
//         // Arrange
//         var scheduleId = 1;
//         var lessonDto = CreateTestLessonDto(1);
//
//         _lessonRepositoryMock
//             .Setup(repo => repo.GetLessonByIdAsync(lessonDto.Id, scheduleId))
//             .ThrowsAsync(new NullReferenceException());
//
//         // Act & Assert
//         await Assert.ThrowsAsync<NullReferenceException>(() => 
//             _lessonServices.DeleteLesson(lessonDto, scheduleId));
//     }
//
//     private static Lesson CreateTestLesson(int id)
//     {
//         var teacher = Schedule.CreateTeacher(
//             teacherId: 1,
//             "John", "Doe", "Smith",
//             ["Math"],
//             ["Group A"]);
//         var subject = Schedule.CreateSchoolSubject("Mathematics");
//         var lessonNumber = Schedule.CreateLessonNumber(1, new TimeSpan(9, 0, 0).ToString());
//         var studyGroup = Schedule.CreateStudyGroup("Group A");
//         var classroom = Schedule.CreateClassroom("Room 101", "First floor");
//         
//         return new Lesson(
//             id,
//             subject,
//             lessonNumber,
//             teacher,
//             studyGroup,
//             classroom,
//             "Test comment"
//         );
//     }
//
//     private static LessonDto CreateTestLessonDto(int id)
//     {
//         return new LessonDto
//         {
//             Id = id,
//             Teacher = new TeacherDto
//             {
//                 Id = 1,
//                 Name = "John",
//                 Surname = "Doe",
//                 Patronymic = "Smith",
//                 SchoolSubjects = ["Math"],
//                 StudyGroups = ["Group A"]
//             },
//             Subject = new SchoolSubjectDto { Name = "Mathematics" },
//             LessonNumber = new LessonNumberDto { Number = 1 },
//             StudyGroup = new StudyGroupDto { Name = "Group A" },
//             Classroom = new ClassroomDto { Name = "Room 101", Description = "First floor" },
//             Comment = "Test comment"
//         };
//     }
//
//     private static Schedule CreateTestSchedule(int id)
//     {
//         return new Schedule(id, "Test Schedule", []);
//     }
// }