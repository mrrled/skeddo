// using Application.DtoModels;
// using Application.Services;
// using Domain;
// using Domain.IRepositories;
// using Domain.Models;
// using Application;
// using Moq;
// using Xunit;
// #pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
//
// namespace Tests.ServicesTests;
//
// public class LessonDraftServicesTests
// {
//     private readonly Mock<ILessonDraftRepository> _lessonDraftRepositoryMock;
//     private readonly Mock<ILessonRepository> _lessonRepositoryMock;
//     private readonly Mock<IScheduleRepository> _scheduleRepositoryMock;
//     private readonly Mock<ISchoolSubjectRepository> _schoolSubjectRepositoryMock;
//     private readonly Mock<ITeacherRepository> _teacherRepositoryMock;
//     private readonly Mock<IStudyGroupRepository> _studyGroupRepositoryMock;
//     private readonly Mock<IClassroomRepository> _classroomRepositoryMock;
//     private readonly Mock<ILessonFactory> _lessonFactoryMock;
//     private readonly Mock<IUnitOfWork> _unitOfWorkMock;
//     private readonly LessonDraftServices _lessonDraftServices;
//
//     public LessonDraftServicesTests()
//     {
//         _lessonDraftRepositoryMock = new Mock<ILessonDraftRepository>();
//         _lessonRepositoryMock = new Mock<ILessonRepository>();
//         _scheduleRepositoryMock = new Mock<IScheduleRepository>();
//         _schoolSubjectRepositoryMock = new Mock<ISchoolSubjectRepository>();
//         _teacherRepositoryMock = new Mock<ITeacherRepository>();
//         _studyGroupRepositoryMock = new Mock<IStudyGroupRepository>();
//         _classroomRepositoryMock = new Mock<IClassroomRepository>();
//         _lessonFactoryMock = new Mock<ILessonFactory>();
//         _unitOfWorkMock = new Mock<IUnitOfWork>();
//
//         _lessonDraftServices = new LessonDraftServices(
//             _lessonDraftRepositoryMock.Object,
//             _lessonRepositoryMock.Object,
//             _scheduleRepositoryMock.Object,
//             _schoolSubjectRepositoryMock.Object,
//             _teacherRepositoryMock.Object,
//             _studyGroupRepositoryMock.Object,
//             _classroomRepositoryMock.Object,
//             _lessonFactoryMock.Object,
//             _unitOfWorkMock.Object
//         );
//     }
//
//     private static LessonDraft CreateLessonDraft(
//         Guid id,
//         Guid scheduleId,
//         SchoolSubject schoolSubject,
//         LessonNumber? lessonNumber = null,
//         Teacher? teacher = null,
//         StudyGroup? studyGroup = null,
//         Classroom? classroom = null,
//         string? comment = null)
//     {
//         return new LessonDraft(
//             id,
//             scheduleId,
//             schoolSubject,
//             lessonNumber,
//             teacher,
//             studyGroup,
//             classroom,
//             null,
//             comment
//         );
//     }
//     
//
//     [Fact]
//     public async Task GetLessonDraftsByScheduleId_ReturnsDtos()
//     {
//         // Arrange
//         var scheduleId = Guid.NewGuid();
//         var mathSubject = new SchoolSubject(Guid.NewGuid(), "Math");
//         var lessonDrafts = new List<LessonDraft>
//         {
//             CreateLessonDraft(Guid.NewGuid(), scheduleId, mathSubject),
//             CreateLessonDraft(Guid.NewGuid(), scheduleId, new SchoolSubject(Guid.NewGuid(), "Physics"))
//         };
//
//         _lessonDraftRepositoryMock
//             .Setup(repo => repo.GetLessonDraftsByScheduleId(scheduleId))
//             .ReturnsAsync(lessonDrafts);
//
//         // Act
//         var result = await _lessonDraftServices.GetLessonDraftsByScheduleId(scheduleId);
//
//         // Assert
//         Assert.NotNull(result);
//         Assert.Equal(2, result.Count);
//         Assert.Equal("Math", result[0].SchoolSubject?.Name);
//         Assert.Equal("Physics", result[1].SchoolSubject?.Name);
//         _lessonDraftRepositoryMock.Verify(repo => repo.GetLessonDraftsByScheduleId(scheduleId), Times.Once);
//     }
//
//     [Fact]
//     public async Task GetLessonDraftById_ReturnsDto()
//     {
//         // Arrange
//         var lessonId = Guid.NewGuid();
//         var scheduleId = Guid.NewGuid();
//         var mathSubject = new SchoolSubject(Guid.NewGuid(), "Math");
//         var lessonNumber = LessonNumber.CreateLessonNumber(1, "8:00-8:45");
//         var teacher = Teacher.CreateTeacher(Guid.NewGuid(), "John", "Doe", "DJ", [], []);
//         var studyGroup = new StudyGroup(Guid.NewGuid(), "Group A", []);
//         var classroom = new Classroom(Guid.NewGuid(), "Room 101", "Description");
//         
//         var lessonDraft = CreateLessonDraft(
//             lessonId,
//             scheduleId,
//             mathSubject,
//             lessonNumber,
//             teacher,
//             studyGroup,
//             classroom,
//             "Test comment"
//         );
//
//         _lessonDraftRepositoryMock
//             .Setup(repo => repo.GetLessonDraftById(lessonId))
//             .ReturnsAsync(lessonDraft);
//
//         // Act
//         var result = await _lessonDraftServices.GetLessonDraftById(lessonId);
//
//         // Assert
//         Assert.NotNull(result);
//         Assert.Equal(lessonId, result.Id);
//         Assert.Equal("Math", result.SchoolSubject?.Name);
//         Assert.Equal(1, result.LessonNumber?.Number);
//         Assert.Equal("John", result.Teacher?.Name);
//         Assert.Equal("Group A", result.StudyGroup?.Name);
//         Assert.Equal("Room 101", result.Classroom?.Name);
//         Assert.Equal("Test comment", result.Comment);
//         _lessonDraftRepositoryMock.Verify(repo => repo.GetLessonDraftById(lessonId), Times.Once);
//     }
//
//     [Fact]
//     public async Task EditDraftLesson_WithMissingClassroom_UpdatesDraftAndReturnsDowngraded()
//     {
//         // Arrange
//         var scheduleId = Guid.NewGuid();
//         var lessonId = Guid.NewGuid();
//         var schoolSubjectId = Guid.NewGuid();
//         var teacherId = Guid.NewGuid();
//         var studyGroupId = Guid.NewGuid();
//         var lessonDraftDto = new LessonDraftDto
//         {
//             Id = lessonId,
//             SchoolSubject = new SchoolSubjectDto { Id = schoolSubjectId, Name = "Physics" },
//             LessonNumber = new LessonNumberDto { Number = 1, Time = "8:00-8:45" },
//             Teacher = new TeacherDto { Id = teacherId },
//             Classroom = null, 
//             StudyGroup = new StudyGroupDto { Id = studyGroupId },
//             Comment = "Updated comment"
//         };
//
//         var mathSubject = new SchoolSubject(Guid.NewGuid(), "Math");
//         var originalLessonDraft = CreateLessonDraft(lessonId, scheduleId, mathSubject);
//         
//         var schedule = Schedule.CreateSchedule(scheduleId, "Test Schedule");
//         schedule.AddLessonDraft(originalLessonDraft);
//
//         var physicsSubject = new SchoolSubject(schoolSubjectId, "Physics");
//         LessonNumber.CreateLessonNumber(1, "8:00-8:45");
//         var teacher = Teacher.CreateTeacher(teacherId, "John", "Doe", "DJ", [], []);
//         var studyGroup = new StudyGroup(studyGroupId, "Group A", []);
//
//         _scheduleRepositoryMock
//             .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
//             .ReturnsAsync(schedule);
//
//         _schoolSubjectRepositoryMock
//             .Setup(repo => repo.GetSchoolSubjectByIdAsync(schoolSubjectId))
//             .ReturnsAsync(physicsSubject);
//
//         _teacherRepositoryMock
//             .Setup(repo => repo.GetTeacherByIdAsync(teacherId))
//             .ReturnsAsync(teacher);
//
//         _studyGroupRepositoryMock
//             .Setup(repo => repo.GetStudyGroupByIdAsync(studyGroupId))
//             .ReturnsAsync(studyGroup);
//
//         _classroomRepositoryMock
//             .Setup(repo => repo.GetClassroomByIdAsync(It.IsAny<Guid>()))
//             .ReturnsAsync((Classroom?)null);
//
//         // Act
//         var result = await _lessonDraftServices.EditDraftLesson(lessonDraftDto, scheduleId);
//
//         // Assert
//         Assert.True(result.IsDraft);
//         Assert.NotNull(result.LessonDraft);
//         Assert.Null(result.Lesson);
//         Assert.Equal(lessonId, result.LessonDraft.Id);
//         _lessonDraftRepositoryMock.Verify(repo => repo.Update(It.IsAny<LessonDraft>()), Times.Once);
//         _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//     }
//
//     [Fact]
//     public async Task EditDraftLesson_WithCompleteData_CreatesLessonAndReturnsSuccess()
//     {
//         // Arrange
//         var scheduleId = Guid.NewGuid();
//         var lessonId = Guid.NewGuid();
//         var schoolSubjectId = Guid.NewGuid();
//         var teacherId = Guid.NewGuid();
//         var classroomId = Guid.NewGuid();
//         var studyGroupId = Guid.NewGuid();
//         var lessonDraftDto = new LessonDraftDto
//         {
//             Id = lessonId,
//             SchoolSubject = new SchoolSubjectDto { Id = schoolSubjectId, Name = "Math" },
//             LessonNumber = new LessonNumberDto { Number = 1, Time = "8:00-8:45" },
//             Teacher = new TeacherDto { Id = teacherId },
//             Classroom = new ClassroomDto { Id = classroomId },
//             StudyGroup = new StudyGroupDto { Id = studyGroupId },
//             Comment = "Complete lesson"
//         };
//
//         var mathSubject = new SchoolSubject(schoolSubjectId, "Math");
//         var originalLessonDraft = CreateLessonDraft(lessonId, scheduleId, mathSubject);
//         
//         var schedule = Schedule.CreateSchedule(scheduleId, "Test Schedule");
//         schedule.AddLessonDraft(originalLessonDraft);
//
//         var lessonNumber = LessonNumber.CreateLessonNumber(1, "8:00-8:45");
//         var teacher = Teacher.CreateTeacher(teacherId, "John", "Doe", "DJ", [], []);
//         var studyGroup = new StudyGroup(studyGroupId, "Group A", []);
//         var classroom = new Classroom(classroomId, "Room 101", "Description");
//
//         _scheduleRepositoryMock
//             .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
//             .ReturnsAsync(schedule);
//
//         _schoolSubjectRepositoryMock
//             .Setup(repo => repo.GetSchoolSubjectByIdAsync(schoolSubjectId))
//             .ReturnsAsync(mathSubject);
//
//         _teacherRepositoryMock
//             .Setup(repo => repo.GetTeacherByIdAsync(teacherId))
//             .ReturnsAsync(teacher);
//
//         _studyGroupRepositoryMock
//             .Setup(repo => repo.GetStudyGroupByIdAsync(studyGroupId))
//             .ReturnsAsync(studyGroup);
//
//         _classroomRepositoryMock
//             .Setup(repo => repo.GetClassroomByIdAsync(classroomId))
//             .ReturnsAsync(classroom);
//
//         var lesson = new Lesson(
//             lessonId,
//             scheduleId,
//             mathSubject,
//             lessonNumber,
//             teacher,
//             studyGroup,
//             classroom,
//             null,
//             "Complete lesson"
//         );
//
//         _lessonFactoryMock
//             .Setup(factory => factory.CreateFromDraft(It.IsAny<LessonDraft>()))
//             .Returns(Result<Lesson>.Success(lesson));
//
//         // Act
//         var result = await _lessonDraftServices.EditDraftLesson(lessonDraftDto, scheduleId);
//
//         // Assert
//         Assert.False(result.IsDraft);
//         Assert.NotNull(result.Lesson);
//         Assert.Null(result.LessonDraft);
//         Assert.Equal(lessonId, result.Lesson.Id);
//         _lessonRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Lesson>(), scheduleId), Times.Once);
//         _lessonDraftRepositoryMock.Verify(repo => repo.Delete(It.IsAny<LessonDraft>()), Times.Once);
//         _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//     }
//
//     [Fact]
//     public async Task EditDraftLesson_WithCompleteDataButFactoryFails_ThrowsException()
//     {
//         // Arrange
//         var scheduleId = Guid.NewGuid();
//         var lessonId = Guid.NewGuid();
//         var schoolSubjectId = Guid.NewGuid();
//         var teacherId = Guid.NewGuid();
//         var classroomId = Guid.NewGuid();
//         var studyGroupId = Guid.NewGuid();
//         var lessonDraftDto = new LessonDraftDto
//         {
//             Id = lessonId,
//             SchoolSubject = new SchoolSubjectDto { Id = schoolSubjectId },
//             LessonNumber = new LessonNumberDto { Number = 1 },
//             Teacher = new TeacherDto { Id = teacherId },
//             Classroom = new ClassroomDto { Id = classroomId },
//             StudyGroup = new StudyGroupDto { Id = studyGroupId }
//         };
//
//         var mathSubject = new SchoolSubject(schoolSubjectId, "Math");
//         var originalLessonDraft = CreateLessonDraft(lessonId, scheduleId, mathSubject);
//         
//         var schedule = Schedule.CreateSchedule(scheduleId, "Test Schedule");
//         schedule.AddLessonDraft(originalLessonDraft);
//
//         _scheduleRepositoryMock
//             .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
//             .ReturnsAsync(schedule);
//
//         _schoolSubjectRepositoryMock
//             .Setup(repo => repo.GetSchoolSubjectByIdAsync(schoolSubjectId))
//             .ReturnsAsync(mathSubject);
//
//         _teacherRepositoryMock
//             .Setup(repo => repo.GetTeacherByIdAsync(teacherId))
//             .ReturnsAsync(Teacher.CreateTeacher(teacherId, "John", "Doe", "DJ", [], []));
//
//         _studyGroupRepositoryMock
//             .Setup(repo => repo.GetStudyGroupByIdAsync(studyGroupId))
//             .ReturnsAsync(new StudyGroup(studyGroupId, "Group A", []));
//
//         _classroomRepositoryMock
//             .Setup(repo => repo.GetClassroomByIdAsync(classroomId))
//             .ReturnsAsync(new Classroom(classroomId, "Room 101", "Description"));
//
//         _lessonFactoryMock
//             .Setup(factory => factory.CreateFromDraft(It.IsAny<LessonDraft>()))
//             .Returns(Result<Lesson>.Failure("Factory failed"));
//
//         // Act & Assert
//         await Assert.ThrowsAsync<Exception>(() =>
//             _lessonDraftServices.EditDraftLesson(lessonDraftDto, scheduleId));
//
//         _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
//     }
//
//     [Fact]
//     public async Task EditDraftLesson_DraftNotFound_ThrowsArgumentException()
//     {
//         // Arrange
//         var scheduleId = Guid.NewGuid();
//         var lessonId = Guid.NewGuid();
//         var lessonDraftDto = new LessonDraftDto { Id = lessonId };
//
//         var schedule = Schedule.CreateSchedule(scheduleId, "Test Schedule");
//
//         _scheduleRepositoryMock
//             .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
//             .ReturnsAsync(schedule);
//
//         // Act & Assert
//         var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
//             _lessonDraftServices.EditDraftLesson(lessonDraftDto, scheduleId));
//
//         Assert.Contains($"{lessonId}", exception.Message);
//         _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
//     }
//
//     [Fact]
//     public async Task DeleteLessonDraft_DeletesDraft()
//     {
//         // Arrange
//         var lessonId = Guid.NewGuid();
//         var lessonDto = new LessonDraftDto { Id = lessonId };
//         var scheduleId = Guid.NewGuid();
//         var mathSubject = new SchoolSubject(Guid.NewGuid(), "Math");
//         var lessonDraft = CreateLessonDraft(lessonId, Guid.NewGuid(), mathSubject);
//
//         _lessonDraftRepositoryMock
//             .Setup(repo => repo.GetLessonDraftById(lessonId))
//             .ReturnsAsync(lessonDraft);
//
//         // Act
//         await _lessonDraftServices.DeleteLessonDraft(lessonDto, scheduleId);
//
//         // Assert
//         _lessonDraftRepositoryMock.Verify(repo => repo.Delete(lessonDraft), Times.Once);
//         _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//     }
//
//     [Fact]
//     public async Task GetLessonDraftById_NotFound_ThrowsException()
//     {
//         // Arrange
//         var id = Guid.NewGuid();
//         _lessonDraftRepositoryMock
//             .Setup(repo => repo.GetLessonDraftById(id))
//             .ThrowsAsync(new InvalidOperationException());
//
//         // Act & Assert
//         await Assert.ThrowsAsync<InvalidOperationException>(() =>
//             _lessonDraftServices.GetLessonDraftById(id));
//     }
//
//     [Fact]
//     public async Task EditDraftLesson_WithAllDataMissingExceptSchoolSubject_UpdatesDraftAndReturnsDowngraded()
//     {
//         // Arrange
//         var scheduleId = Guid.NewGuid();
//         var lessonId = Guid.NewGuid();
//         var schoolSubjectId = Guid.NewGuid();
//         var lessonDraftDto = new LessonDraftDto
//         {
//             Id = lessonId,
//             SchoolSubject = new SchoolSubjectDto { Id = schoolSubjectId, Name = "Math" },
//             LessonNumber = null,
//             Teacher = null,
//             Classroom = null,
//             StudyGroup = null,
//             Comment = "Incomplete data"
//         };
//
//         var mathSubject = new SchoolSubject(schoolSubjectId, "Math");
//         var originalLessonDraft = CreateLessonDraft(lessonId, scheduleId, mathSubject);
//         
//         var schedule = Schedule.CreateSchedule(scheduleId, "Test Schedule");
//         schedule.AddLessonDraft(originalLessonDraft);
//
//         _scheduleRepositoryMock
//             .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
//             .ReturnsAsync(schedule);
//
//         _schoolSubjectRepositoryMock
//             .Setup(repo => repo.GetSchoolSubjectByIdAsync(schoolSubjectId))
//             .ReturnsAsync(mathSubject);
//
//         // Act
//         var result = await _lessonDraftServices.EditDraftLesson(lessonDraftDto, scheduleId);
//
//         // Assert
//         Assert.True(result.IsDraft);
//         Assert.NotNull(result.LessonDraft);
//         Assert.Null(result.Lesson);
//         _lessonDraftRepositoryMock.Verify(repo => repo.Update(It.IsAny<LessonDraft>()), Times.Once);
//         _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//     }
//
//     [Fact]
//     public async Task EditDraftLesson_WithNullSchoolSubject_ThrowsException()
//     {
//         // Arrange
//         var scheduleId = Guid.NewGuid();
//         var lessonId = Guid.NewGuid();
//         var lessonDraftDto = new LessonDraftDto
//         {
//             Id = lessonId,
//             SchoolSubject = null,
//             LessonNumber = new LessonNumberDto { Number = 1 },
//             Teacher = new TeacherDto { Id = Guid.NewGuid() },
//             Classroom = new ClassroomDto { Id = Guid.NewGuid() },
//             StudyGroup = new StudyGroupDto { Id = Guid.NewGuid() }
//         };
//
//         var mathSubject = new SchoolSubject(Guid.NewGuid(), "Math");
//         var originalLessonDraft = CreateLessonDraft(lessonId, scheduleId, mathSubject);
//         
//         var schedule = Schedule.CreateSchedule(scheduleId, "Test Schedule");
//         schedule.AddLessonDraft(originalLessonDraft);
//
//         _scheduleRepositoryMock
//             .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
//             .ReturnsAsync(schedule);
//
//         // Act & Assert
//         await Assert.ThrowsAsync<ArgumentNullException>(() =>
//             _lessonDraftServices.EditDraftLesson(lessonDraftDto, scheduleId));
//     }
//
//     [Fact]
//     public async Task EditDraftLesson_ReturnsDraftWhenAnyRequiredPropertyIsNull()
//     {
//         // Arrange
//         var scheduleId = Guid.NewGuid();
//         var schoolSubjectId = Guid.NewGuid();
//         var classroomId = Guid.NewGuid();
//         var studyGroupId = Guid.NewGuid();
//         var lessonId = Guid.NewGuid();
//         var lessonDraftDto = new LessonDraftDto
//         {
//             Id = lessonId,
//             SchoolSubject = new SchoolSubjectDto { Id = schoolSubjectId, Name = "Math" },
//             LessonNumber = new LessonNumberDto { Number = 1, Time = "8:00-8:45" },
//             Teacher = null, // Missing teacher
//             Classroom = new ClassroomDto { Id = classroomId },
//             StudyGroup = new StudyGroupDto { Id = studyGroupId }
//         };
//
//         var mathSubject = new SchoolSubject(schoolSubjectId, "Math");
//         var originalLessonDraft = CreateLessonDraft(lessonId, scheduleId, mathSubject);
//         
//         var schedule = Schedule.CreateSchedule(scheduleId, "Test Schedule");
//         schedule.AddLessonDraft(originalLessonDraft);
//
//         LessonNumber.CreateLessonNumber(1, "8:00-8:45");
//         var classroom = new Classroom(classroomId, "Room 101", "Description");
//         var studyGroup = new StudyGroup(studyGroupId, "Group A", []);
//
//         _scheduleRepositoryMock
//             .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
//             .ReturnsAsync(schedule);
//
//         _schoolSubjectRepositoryMock
//             .Setup(repo => repo.GetSchoolSubjectByIdAsync(schoolSubjectId))
//             .ReturnsAsync(mathSubject);
//
//         _teacherRepositoryMock
//             .Setup(repo => repo.GetTeacherByIdAsync(It.IsAny<Guid>()))
//             .ReturnsAsync((Teacher?)null);
//
//         _studyGroupRepositoryMock
//             .Setup(repo => repo.GetStudyGroupByIdAsync(studyGroupId))
//             .ReturnsAsync(studyGroup);
//
//         _classroomRepositoryMock
//             .Setup(repo => repo.GetClassroomByIdAsync(classroomId))
//             .ReturnsAsync(classroom);
//
//         // Act
//         var result = await _lessonDraftServices.EditDraftLesson(lessonDraftDto, scheduleId);
//
//         // Assert
//         Assert.True(result.IsDraft);
//         Assert.NotNull(result.LessonDraft);
//         Assert.Null(result.Lesson);
//         _lessonDraftRepositoryMock.Verify(repo => repo.Update(It.IsAny<LessonDraft>()), Times.Once);
//         _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//     }
// }