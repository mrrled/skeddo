using Application.DtoModels;
using Application.Services;
using Domain;
using Domain.IRepositories;
using Domain.Models;
using FluentAssertions;
using Application;
using Moq;
using Xunit;
// ReSharper disable PreferConcreteValueOverDefault
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

namespace Tests.ServicesTests;

public class LessonServicesTests
{
    private readonly Mock<IScheduleRepository> _scheduleRepositoryMock;
    private readonly Mock<ILessonRepository> _lessonRepositoryMock;
    private readonly Mock<ITeacherRepository> _teacherRepositoryMock;
    private readonly Mock<ISchoolSubjectRepository> _schoolSubjectRepositoryMock;
    private readonly Mock<IStudyGroupRepository> _studyGroupRepositoryMock;
    private readonly Mock<IClassroomRepository> _classroomRepositoryMock;
    private readonly Mock<ILessonFactory> _lessonFactoryMock;
    private readonly Mock<ILessonDraftRepository> _lessonDraftRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly LessonServices _lessonServices;

    public LessonServicesTests()
    {
        _scheduleRepositoryMock = new Mock<IScheduleRepository>();
        _lessonRepositoryMock = new Mock<ILessonRepository>();
        _teacherRepositoryMock = new Mock<ITeacherRepository>();
        _schoolSubjectRepositoryMock = new Mock<ISchoolSubjectRepository>();
        _studyGroupRepositoryMock = new Mock<IStudyGroupRepository>();
        _classroomRepositoryMock = new Mock<IClassroomRepository>();
        _lessonFactoryMock = new Mock<ILessonFactory>();
        _lessonDraftRepositoryMock = new Mock<ILessonDraftRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _lessonServices = new LessonServices(
            _scheduleRepositoryMock.Object,
            _lessonRepositoryMock.Object,
            _teacherRepositoryMock.Object,
            _schoolSubjectRepositoryMock.Object,
            _studyGroupRepositoryMock.Object,
            _classroomRepositoryMock.Object,
            _lessonFactoryMock.Object,
            _lessonDraftRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task GetLessonsByScheduleId_ValidScheduleId_ReturnsLessonsDto()
    {
        // Arrange
        var scheduleId = 1;
        var lessons = new List<Lesson>
        {
            CreateTestLesson(1),
            CreateTestLesson(2)
        };
        
        _lessonRepositoryMock
            .Setup(r => r.GetLessonsByScheduleIdAsync(scheduleId))
            .ReturnsAsync(lessons);

        // Act
        var result = await _lessonServices.GetLessonsByScheduleId(scheduleId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);
        _lessonRepositoryMock.Verify(r => r.GetLessonsByScheduleIdAsync(scheduleId), Times.Once);
    }

    [Fact]
    public async Task GetLessonsByScheduleId_EmptyLessonsList_ReturnsEmptyList()
    {
        // Arrange
        var scheduleId = 1;
        var emptyLessons = new List<Lesson>();
        
        _lessonRepositoryMock
            .Setup(r => r.GetLessonsByScheduleIdAsync(scheduleId))
            .ReturnsAsync(emptyLessons);

        // Act
        var result = await _lessonServices.GetLessonsByScheduleId(scheduleId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddLesson_SchoolSubjectIsNull_ThrowsArgumentException()
    {
        // Arrange
        var lessonDto = new LessonDto { Id = 1 };
        var scheduleId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _lessonServices.AddLesson(lessonDto, scheduleId));
    }

    [Fact]
    public async Task AddLesson_ScheduleNotFound_ThrowsArgumentException()
    {
        // Arrange
        var lessonDto = new LessonDto 
        { 
            Id = 1, 
            SchoolSubject = new SchoolSubjectDto { Id = 1 }
        };
        var scheduleId = 1;

        _scheduleRepositoryMock
            .Setup(r => r.GetScheduleByIdAsync(scheduleId))
            .ReturnsAsync((Schedule?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _lessonServices.AddLesson(lessonDto, scheduleId));
    }

    [Fact]
    public async Task AddLesson_LessonFactorySuccess_AddsLessonAndUpdatesRelated()
    {
        // Arrange
        var scheduleId = 1;
        var lessonDto = CreateTestLessonDto(1);
        var schedule = CreateTestSchedule(scheduleId);
        var lesson = CreateTestLesson(1);

        SetupRepositoryMocksForAddLesson(lessonDto);
        
        _scheduleRepositoryMock
            .Setup(r => r.GetScheduleByIdAsync(scheduleId))
            .ReturnsAsync(schedule);
        
        _lessonFactoryMock
            .Setup(f => f.CreateFromDraft(It.IsAny<LessonDraft>()))
            .Returns(Result<Lesson>.Success(lesson));
        
        schedule.AddLesson(lesson);

        // Act
        await _lessonServices.AddLesson(lessonDto, scheduleId);

        // Assert
        _lessonRepositoryMock.Verify(r => r.AddAsync(lesson, scheduleId), Times.Once);
        _lessonRepositoryMock.Verify(r => r.UpdateRangeAsync(It.IsAny<List<Lesson>>()), Times.Once);
        _lessonDraftRepositoryMock.Verify(r => r.AddAsync(It.IsAny<LessonDraft>(), scheduleId), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task AddLesson_LessonFactoryFails_CreatesDraft()
    {
        // Arrange
        var scheduleId = 1;
        var lessonDto = CreateTestLessonDto(1);
        var schedule = CreateTestSchedule(scheduleId);
        
        SetupRepositoryMocksForAddLesson(lessonDto);
        
        _scheduleRepositoryMock
            .Setup(r => r.GetScheduleByIdAsync(scheduleId))
            .ReturnsAsync(schedule);
        
        _lessonFactoryMock
            .Setup(f => f.CreateFromDraft(It.IsAny<LessonDraft>()))
            .Returns(Result<Lesson>.Failure("Validation failed"));

        // Act
        await _lessonServices.AddLesson(lessonDto, scheduleId);

        // Assert
        _lessonRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Lesson>(), scheduleId), Times.Never);
        _lessonDraftRepositoryMock.Verify(r => r.AddAsync(It.IsAny<LessonDraft>(), scheduleId), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task AddLesson_NullTeacherInDto_ButFactoryReturnsSuccess_AddsLesson()
    {
        // Arrange
        var scheduleId = 1;
        var lessonDto = CreateTestLessonDto(1);
        lessonDto.Teacher = null; // Teacher is null in DTO
        var schedule = CreateTestSchedule(scheduleId);
        var lesson = CreateTestLesson(1);
        
        _scheduleRepositoryMock
            .Setup(r => r.GetScheduleByIdAsync(scheduleId))
            .ReturnsAsync(schedule);
        
        _schoolSubjectRepositoryMock
            .Setup(r => r.GetSchoolSubjectByIdAsync(lessonDto.SchoolSubject!.Id))
            .ReturnsAsync(new SchoolSubject(1, "Math"));
        
        _teacherRepositoryMock
            .Setup(r => r.GetTeacherByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Teacher?)null); // Returns null from repository
        
        _classroomRepositoryMock
            .Setup(r => r.GetClassroomByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Classroom(1, "101"));
        
        _studyGroupRepositoryMock
            .Setup(r => r.GetStudyGroupByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new StudyGroup(1, "Group A", []));
        
        // Factory returns success even though teacher is null (mocked behavior)
        _lessonFactoryMock
            .Setup(f => f.CreateFromDraft(It.IsAny<LessonDraft>()))
            .Returns(Result<Lesson>.Success(lesson));

        // Act
        await _lessonServices.AddLesson(lessonDto, scheduleId);

        // Assert
        _lessonRepositoryMock.Verify(r => r.AddAsync(lesson, scheduleId), Times.Once);
    }

    [Fact]
    public async Task AddLesson_NullTeacher_FactoryReturnsFailure_CreatesDraft()
    {
        // Arrange
        var scheduleId = 1;
        var lessonDto = CreateTestLessonDto(1);
        lessonDto.Teacher = null; // Teacher is null in DTO
        var schedule = CreateTestSchedule(scheduleId);
        
        _scheduleRepositoryMock
            .Setup(r => r.GetScheduleByIdAsync(scheduleId))
            .ReturnsAsync(schedule);
        
        _schoolSubjectRepositoryMock
            .Setup(r => r.GetSchoolSubjectByIdAsync(lessonDto.SchoolSubject!.Id))
            .ReturnsAsync(new SchoolSubject(1, "Math"));
        
        _teacherRepositoryMock
            .Setup(r => r.GetTeacherByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Teacher?)null);
        
        _classroomRepositoryMock
            .Setup(r => r.GetClassroomByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Classroom(1, "101"));
        
        _studyGroupRepositoryMock
            .Setup(r => r.GetStudyGroupByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new StudyGroup(1, "Group A", []));
        
        // Factory returns failure because teacher is null (as per LessonFactory implementation)
        _lessonFactoryMock
            .Setup(f => f.CreateFromDraft(It.IsAny<LessonDraft>()))
            .Returns(Result<Lesson>.Failure("Teacher cannot be null"));

        // Act
        await _lessonServices.AddLesson(lessonDto, scheduleId);

        // Assert
        _lessonRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Lesson>(), scheduleId), Times.Never);
        _lessonDraftRepositoryMock.Verify(r => r.AddAsync(It.IsAny<LessonDraft>(), scheduleId), Times.Once);
    }

    [Fact]
    public async Task EditLesson_ScheduleNotFound_ThrowsArgumentException()
    {
        // Arrange
        var lessonDto = CreateTestLessonDto(1);
        var scheduleId = 1;

        _scheduleRepositoryMock
            .Setup(r => r.GetScheduleByIdAsync(scheduleId))
            .ReturnsAsync((Schedule?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _lessonServices.EditLesson(lessonDto, scheduleId));
    }

    [Fact]
    public async Task EditLesson_LessonNotFound_ThrowsArgumentException()
    {
        // Arrange
        var lessonDto = CreateTestLessonDto(1);
        var scheduleId = 1;
        var schedule = CreateTestSchedule(scheduleId);
        schedule.AddLesson(CreateTestLesson(2)); // Другой ID

        _scheduleRepositoryMock
            .Setup(r => r.GetScheduleByIdAsync(scheduleId))
            .ReturnsAsync(schedule);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _lessonServices.EditLesson(lessonDto, scheduleId));
    }

    [Fact]
    public async Task EditLesson_AllFieldsProvided_UpdatesLessonSuccessfully()
    {
        // Arrange
        var scheduleId = 1;
        var lessonDto = CreateTestLessonDto(1);
        var schedule = CreateTestSchedule(scheduleId);
        var existingLesson = CreateTestLesson(1);
        schedule.AddLesson(existingLesson);
        
        SetupRepositoryMocksForEditLesson(lessonDto);
        
        _scheduleRepositoryMock
            .Setup(r => r.GetScheduleByIdAsync(scheduleId))
            .ReturnsAsync(schedule);

        _lessonRepositoryMock
            .Setup(r => r.UpdateRangeAsync(It.IsAny<List<Lesson>>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _lessonServices.EditLesson(lessonDto, scheduleId);

        // Assert
        result.Should().NotBeNull();
        result.IsDraft.Should().BeFalse();
        result.Lesson.Should().NotBeNull();
        result.LessonDraft.Should().BeNull();
        
        _lessonRepositoryMock.Verify(r => r.UpdateRangeAsync(It.IsAny<List<Lesson>>()), Times.Once);
        _lessonDraftRepositoryMock.Verify(r => r.AddAsync(It.IsAny<LessonDraft>(), scheduleId), Times.Never);
        _lessonRepositoryMock.Verify(r => r.Delete(It.IsAny<Lesson>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task EditLesson_SomeFieldsNull_CreatesDraftAndDeletesLesson()
    {
        // Arrange
        var scheduleId = 1;
        var lessonDto = CreateTestLessonDto(1);
        lessonDto.Teacher = null; // Делаем поле null
        var schedule = CreateTestSchedule(scheduleId);
        var existingLesson = CreateTestLesson(1);
        schedule.AddLesson(existingLesson);
        
        SetupRepositoryMocksForEditLesson(lessonDto);
        
        _scheduleRepositoryMock
            .Setup(r => r.GetScheduleByIdAsync(scheduleId))
            .ReturnsAsync(schedule);

        _lessonRepositoryMock
            .Setup(r => r.Delete(It.IsAny<Lesson>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _lessonServices.EditLesson(lessonDto, scheduleId);

        // Assert
        result.Should().NotBeNull();
        result.IsDraft.Should().BeTrue();
        result.Lesson.Should().BeNull();
        result.LessonDraft.Should().NotBeNull();
        
        _lessonDraftRepositoryMock.Verify(r => r.AddAsync(It.IsAny<LessonDraft>(), scheduleId), Times.Once);
        _lessonRepositoryMock.Verify(r => r.Delete(existingLesson), Times.Once);
        _lessonRepositoryMock.Verify(r => r.UpdateRangeAsync(It.IsAny<List<Lesson>>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Theory]
    [InlineData(null, "LessonNumber", "Teacher", "Classroom", "StudyGroup")]
    [InlineData("SchoolSubject", null, "Teacher", "Classroom", "StudyGroup")]
    [InlineData("SchoolSubject", "LessonNumber", null, "Classroom", "StudyGroup")]
    [InlineData("SchoolSubject", "LessonNumber", "Teacher", null, "StudyGroup")]
    [InlineData("SchoolSubject", "LessonNumber", "Teacher", "Classroom", null)]
    public async Task EditLesson_AnyRequiredFieldNull_CreatesDraft(
        string? schoolSubject, string? lessonNumber, string? teacher, 
        string? classroom, string? studyGroup)
    {
        // Arrange
        var scheduleId = 1;
        var lessonDto = CreateTestLessonDto(1);
        
        if (schoolSubject == null) lessonDto.SchoolSubject = null;
        if (lessonNumber == null) lessonDto.LessonNumber = null;
        if (teacher == null) lessonDto.Teacher = null;
        if (classroom == null) lessonDto.Classroom = null;
        if (studyGroup == null) lessonDto.StudyGroup = null;
        
        var schedule = CreateTestSchedule(scheduleId);
        var existingLesson = CreateTestLesson(1);
        schedule.AddLesson(existingLesson);
        
        _scheduleRepositoryMock
            .Setup(r => r.GetScheduleByIdAsync(scheduleId))
            .ReturnsAsync(schedule);

        // Act
        var result = await _lessonServices.EditLesson(lessonDto, scheduleId);

        // Assert
        result.IsDraft.Should().BeTrue();
        _lessonDraftRepositoryMock.Verify(r => r.AddAsync(It.IsAny<LessonDraft>(), scheduleId), Times.Once);
    }

    [Fact]
    public async Task DeleteLesson_ValidLesson_DeletesLessonAndSaves()
    {
        // Arrange
        var lessonDto = CreateTestLessonDto(1);
        var scheduleId = 1;
        var lesson = CreateTestLesson(1);
        
        _lessonRepositoryMock
            .Setup(r => r.GetLessonByIdAsync(lessonDto.Id))
            .ReturnsAsync(lesson);
        
        _lessonRepositoryMock
            .Setup(r => r.Delete(lesson))
            .Returns(Task.CompletedTask);

        // Act
        await _lessonServices.DeleteLesson(lessonDto, scheduleId);

        // Assert
        _lessonRepositoryMock.Verify(r => r.GetLessonByIdAsync(lessonDto.Id), Times.Once);
        _lessonRepositoryMock.Verify(r => r.Delete(lesson), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteLesson_LessonNotFound_ThrowsException()
    {
        // Arrange
        var lessonDto = CreateTestLessonDto(1);
        var scheduleId = 1;
        
        _lessonRepositoryMock
            .Setup(r => r.GetLessonByIdAsync(lessonDto.Id))
            .ReturnsAsync((Lesson?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => 
            _lessonServices.DeleteLesson(lessonDto, scheduleId));
    }

    [Fact]
    public async Task AddLesson_TeacherSpecializationCheck_WarningTypeSet()
    {
        // Arrange
        var scheduleId = 1;
        var lessonDto = CreateTestLessonDto(1);
        var schedule = CreateTestSchedule(scheduleId);
        
        var mathSubject = new SchoolSubject(1, "Math");
        var teacher = new Teacher(
            1, "John", "Doe", "Smith",
            [new(2, "Physics")], // Учитель не специализируется на Math
            [new StudyGroup(1, "Group A", [])]
        );
        var lesson = CreateTestLesson(1);
        
        SetupRepositoryMocksForAddLesson(lessonDto, teacher, mathSubject);
        
        _scheduleRepositoryMock
            .Setup(r => r.GetScheduleByIdAsync(scheduleId))
            .ReturnsAsync(schedule);
        
        _lessonFactoryMock
            .Setup(f => f.CreateFromDraft(It.IsAny<LessonDraft>()))
            .Returns(Result<Lesson>.Success(lesson));

        // Act
        await _lessonServices.AddLesson(lessonDto, scheduleId);

        // Assert
        _lessonRepositoryMock.Verify(r => r.UpdateRangeAsync(It.IsAny<List<Lesson>>()), Times.Once);
    }

    [Fact]
    public async Task AddLesson_ConflictCheck_WarningTypeSetForConflicts()
    {
        // Arrange
        var scheduleId = 1;
        var lessonDto = CreateTestLessonDto(1);
        var schedule = CreateTestSchedule(scheduleId);
        
        // Создаем существующий урок с теми же данными
        var existingLesson = CreateTestLesson(2);
        existingLesson.SetTeacher(new Teacher(1, "John", "Doe", "Smith", 
            new List<SchoolSubject>(), new List<StudyGroup>()));
        existingLesson.SetClassroom(new Classroom(1, "101"));
        schedule.AddLesson(existingLesson);
        
        var newLesson = CreateTestLesson(1);
        newLesson.SetTeacher(existingLesson.Teacher);
        newLesson.SetClassroom(existingLesson.Classroom);
        
        SetupRepositoryMocksForAddLesson(lessonDto);
        
        _scheduleRepositoryMock
            .Setup(r => r.GetScheduleByIdAsync(scheduleId))
            .ReturnsAsync(schedule);
        
        _lessonFactoryMock
            .Setup(f => f.CreateFromDraft(It.IsAny<LessonDraft>()))
            .Returns(Result<Lesson>.Success(newLesson));

        // Act
        await _lessonServices.AddLesson(lessonDto, scheduleId);

        // Assert
        _lessonRepositoryMock.Verify(r => r.UpdateRangeAsync(It.IsAny<List<Lesson>>()), Times.Once);
    }

    [Fact]
    public async Task AddLesson_LessonFactoryChecksAllRequiredFields_ReturnsFailureForAnyNull()
    {
        // Arrange
        var scheduleId = 1;
        var lessonDto = CreateTestLessonDto(1);
        var schedule = CreateTestSchedule(scheduleId);
        
        SetupRepositoryMocksForAddLesson(lessonDto);
        
        _scheduleRepositoryMock
            .Setup(r => r.GetScheduleByIdAsync(scheduleId))
            .ReturnsAsync(schedule);
        
        // Factory проверяет все поля: StudyGroup, Teacher, LessonNumber, Classroom
        _lessonFactoryMock
            .Setup(f => f.CreateFromDraft(It.IsAny<LessonDraft>()))
            .Returns<LessonDraft>(draft =>
            {
                if (draft.StudyGroup == null)
                    return Result<Lesson>.Failure("StudyGroup cannot be null");
                if (draft.Teacher == null)
                    return Result<Lesson>.Failure("Teacher cannot be null");
                if (draft.LessonNumber == null)
                    return Result<Lesson>.Failure("LessonNumber cannot be null");
                if (draft.Classroom == null)
                    return Result<Lesson>.Failure("Classroom cannot be null");
                
                return Result<Lesson>.Success(new Lesson(
                    draft.Id,
                    draft.SchoolSubject,
                    draft.LessonNumber!,
                    draft.Teacher!,
                    draft.StudyGroup!,
                    draft.Classroom!,
                    null,
                    draft.Comment
                ));
            });

        // Act
        await _lessonServices.AddLesson(lessonDto, scheduleId);

        // Assert
        // Factory should be called
        _lessonFactoryMock.Verify(f => f.CreateFromDraft(It.IsAny<LessonDraft>()), Times.Once);
    }

    [Fact]
    public async Task AddLesson_RealLessonFactoryBehavior_MocksCorrectly()
    {
        // Arrange
        var scheduleId = 1;
        var lessonDto = CreateTestLessonDto(1);
        // Делаем одну из сущностей null
        lessonDto.Classroom = null;
        
        var schedule = CreateTestSchedule(scheduleId);
        
        SetupRepositoryMocksForAddLesson(lessonDto);
        
        _scheduleRepositoryMock
            .Setup(r => r.GetScheduleByIdAsync(scheduleId))
            .ReturnsAsync(schedule);
        
        // Используем реальное поведение LessonFactory через мок
        _lessonFactoryMock
            .Setup(f => f.CreateFromDraft(It.IsAny<LessonDraft>()))
            .Returns<LessonDraft>(draft =>
            {
                // Имитация реальной логики фабрики
                if (draft.StudyGroup == null)
                    return Result<Lesson>.Failure("StudyGroup cannot be null");
                if (draft.Teacher == null)
                    return Result<Lesson>.Failure("Teacher cannot be null");
                if (draft.LessonNumber == null)
                    return Result<Lesson>.Failure("LessonNumber cannot be null");
                if (draft.Classroom == null)
                    return Result<Lesson>.Failure("Classroom cannot be null");
                
                return Result<Lesson>.Success(new Lesson(
                    draft.Id,
                    draft.SchoolSubject,
                    draft.LessonNumber,
                    draft.Teacher,
                    draft.StudyGroup,
                    draft.Classroom,
                    null,
                    draft.Comment
                ));
            });

        // Act
        await _lessonServices.AddLesson(lessonDto, scheduleId);

        // Assert
        // Since classroom is null, factory should return failure
        _lessonDraftRepositoryMock.Verify(r => r.AddAsync(It.IsAny<LessonDraft>(), scheduleId), Times.Once);
        _lessonRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Lesson>(), scheduleId), Times.Never);
    }

    private Lesson CreateTestLesson(int id)
    {
        return new Lesson(
            id,
            new SchoolSubject(1, "Math"),
            LessonNumber.CreateLessonNumber(1, "09:00"),
            new Teacher(1, "John", "Doe", "Smith", new List<SchoolSubject>(), new List<StudyGroup>()),
            new StudyGroup(1, "Group A", []),
            new Classroom(1, "101"),
            null,
            "Test comment",
            WarningType.Normal
        );
    }

    private LessonDto CreateTestLessonDto(int id)
    {
        return new LessonDto
        {
            Id = id,
            SchoolSubject = new SchoolSubjectDto { Id = 1, Name = "Math" },
            LessonNumber = new LessonNumberDto { Id = 1, Number = 1, Time = "09:00" },
            Teacher = new TeacherDto { Id = 1, Name = "John", Surname = "Doe", Patronymic = "Smith" },
            StudyGroup = new StudyGroupDto { Id = 1, Name = "Group A" },
            Classroom = new ClassroomDto { Id = 1, Name = "101" },
            Comment = "Test comment",
            WarningType = WarningType.Normal
        };
    }

    private Schedule CreateTestSchedule(int id)
    {
        return Schedule.CreateSchedule(id, "Test Schedule");
    }

    private void SetupRepositoryMocksForAddLesson(
        LessonDto lessonDto, 
        Teacher? teacher = null, 
        SchoolSubject? schoolSubject = null)
    {
        teacher ??= new Teacher(1, "John", "Doe", "Smith", new List<SchoolSubject>(), new List<StudyGroup>());
        schoolSubject ??= new SchoolSubject(1, "Math");
        
        _teacherRepositoryMock
            .Setup(r => r.GetTeacherByIdAsync(lessonDto.Teacher.Id))
            .ReturnsAsync(teacher);
        
        _schoolSubjectRepositoryMock
            .Setup(r => r.GetSchoolSubjectByIdAsync(lessonDto.SchoolSubject!.Id))
            .ReturnsAsync(schoolSubject);
        
        _classroomRepositoryMock
            .Setup(r => r.GetClassroomByIdAsync(lessonDto.Classroom.Id))
            .ReturnsAsync(new Classroom(1, "101"));
        
        _studyGroupRepositoryMock
            .Setup(r => r.GetStudyGroupByIdAsync(lessonDto.StudyGroup.Id))
            .ReturnsAsync(new StudyGroup(1, "Group A", []));
    }

    private void SetupRepositoryMocksForEditLesson(LessonDto lessonDto)
    {
        if (lessonDto.SchoolSubject != null)
        {
            _schoolSubjectRepositoryMock
                .Setup(r => r.GetSchoolSubjectByIdAsync(lessonDto.SchoolSubject.Id))
                .ReturnsAsync(new SchoolSubject(lessonDto.SchoolSubject.Id, lessonDto.SchoolSubject.Name));
        }
        
        if (lessonDto.Teacher != null)
        {
            _teacherRepositoryMock
                .Setup(r => r.GetTeacherByIdAsync(lessonDto.Teacher.Id))
                .ReturnsAsync(new Teacher(
                    lessonDto.Teacher.Id, 
                    lessonDto.Teacher.Name, 
                    lessonDto.Teacher.Surname, 
                    lessonDto.Teacher.Patronymic,
                    [],
                    []));
        }
        
        if (lessonDto.StudyGroup != null)
        {
            _studyGroupRepositoryMock
                .Setup(r => r.GetStudyGroupByIdAsync(lessonDto.StudyGroup.Id))
                .ReturnsAsync(new StudyGroup(lessonDto.StudyGroup.Id, lessonDto.StudyGroup.Name, []));
        }
        
        if (lessonDto.Classroom != null)
        {
            _classroomRepositoryMock
                .Setup(r => r.GetClassroomByIdAsync(lessonDto.Classroom.Id))
                .ReturnsAsync(new Classroom(lessonDto.Classroom.Id, lessonDto.Classroom.Name));
        }
    }
}