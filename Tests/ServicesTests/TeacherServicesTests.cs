using Application.DtoModels;
using Application;
using Application.Services;
using Domain.IRepositories;
using Domain.Models;
using Moq;
using Xunit;
// ReSharper disable PreferConcreteValueOverDefault

namespace Tests.ServicesTests;

public class TeacherServicesTests
{
    private readonly Mock<ITeacherRepository> _mockTeacherRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly TeacherServices _teacherServices;
    
    public TeacherServicesTests()
    {
        _mockTeacherRepository = new Mock<ITeacherRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _teacherServices = new TeacherServices(_mockTeacherRepository.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task FetchTeachersFromBackendAsync_ShouldReturnListOfTeacherDtos()
    {
        // Arrange
        var teachers = new List<Teacher>
        {
            Schedule.CreateTeacher(1, "Иван", "Иванов", "Иванович", [], []),
            Schedule.CreateTeacher(2, "Петр", "Петров", "Петрович", [], [])
        };
        
        _mockTeacherRepository
            .Setup(repo => repo.GetTeacherListAsync())
            .ReturnsAsync(teachers);

        // Act
        var result = await _teacherServices.FetchTeachersFromBackendAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Иван", result[0].Name);
        Assert.Equal("Петров", result[1].Surname);
        _mockTeacherRepository.Verify(repo => repo.GetTeacherListAsync(), Times.Once);
    }

    [Fact]
    public async Task GetTeacherById_ValidId_ShouldReturnTeacherDto()
    {
        // Arrange
        var teacherId = 1;
        var teacher = Schedule.CreateTeacher(teacherId, "Иван", "Иванов", "Иванович", 
            [], []);
        
        _mockTeacherRepository
            .Setup(repo => repo.GetTeacherByIdAsync(teacherId))
            .ReturnsAsync(teacher);

        // Act
        var result = await _teacherServices.GetTeacherById(teacherId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(teacherId, result.Id);
        Assert.Equal("Иван", result.Name);
        Assert.Equal("Иванов", result.Surname);
        _mockTeacherRepository.Verify(repo => repo.GetTeacherByIdAsync(teacherId), Times.Once);
    }

    [Fact]
    public async Task AddTeacher_ValidDto_ShouldCallRepositoryAndSaveChanges()
    {
        // Arrange
        var teacherDto = new TeacherDto
        {
            Id = 1,
            Name = "Иван",
            Surname = "Иванов",
            Patronymic = "Иванович",
            SchoolSubjects = [],
            StudyGroups = []
        };

        _mockTeacherRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Teacher>()))
            .Returns(Task.CompletedTask);
        
        _mockUnitOfWork
            .Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        await _teacherServices.AddTeacher(teacherDto);

        // Assert
        _mockTeacherRepository.Verify(repo => repo.AddAsync(It.Is<Teacher>(t => 
            t.Id == teacherDto.Id && 
            t.Name == teacherDto.Name)), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task EditTeacher_ValidDto_ShouldUpdateTeacherAndSaveChanges()
    {
        // Arrange
        var teacherDto = new TeacherDto
        {
            Id = 1,
            Name = "НовоеИмя",
            Surname = "НоваяФамилия",
            Patronymic = "НовоеОтчество",
            SchoolSubjects = [],
            StudyGroups = []
        };

        var existingTeacher = Schedule.CreateTeacher(1, "СтароеИмя", "СтараяФамилия", "СтароеОтчество",
            [], []);
        
        _mockTeacherRepository
            .Setup(repo => repo.GetTeacherByIdAsync(teacherDto.Id))
            .ReturnsAsync(existingTeacher);
        
        _mockTeacherRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<Teacher>()))
            .Returns(Task.CompletedTask);
        
        _mockUnitOfWork
            .Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        await _teacherServices.EditTeacher(teacherDto);

        // Assert
        _mockTeacherRepository.Verify(repo => repo.GetTeacherByIdAsync(teacherDto.Id), Times.Once);
        _mockTeacherRepository.Verify(repo => repo.UpdateAsync(It.Is<Teacher>(t => 
            t.Id == teacherDto.Id)), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteTeacher_ValidDto_ShouldDeleteTeacherAndSaveChanges()
    {
        // Arrange
        var teacherDto = new TeacherDto { Id = 1 };
        
        var existingTeacher = Schedule.CreateTeacher(1, "Иван", "Иванов", "Иванович",
            [], []);
        
        _mockTeacherRepository
            .Setup(repo => repo.GetTeacherByIdAsync(teacherDto.Id))
            .ReturnsAsync(existingTeacher);
        
        _mockTeacherRepository
            .Setup(repo => repo.Delete(It.IsAny<Teacher>()))
            .Returns(Task.CompletedTask);
        
        _mockUnitOfWork
            .Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        await _teacherServices.DeleteTeacher(teacherDto);

        // Assert
        _mockTeacherRepository.Verify(repo => repo.GetTeacherByIdAsync(teacherDto.Id), Times.Once);
        _mockTeacherRepository.Verify(repo => repo.Delete(It.Is<Teacher>(t => 
            t.Id == teacherDto.Id)), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task GetTeacherById_RepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var teacherId = 999;
        
        _mockTeacherRepository
            .Setup(repo => repo.GetTeacherByIdAsync(teacherId))
            .ThrowsAsync(new NullReferenceException("Учитель не найден"));

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => 
            _teacherServices.GetTeacherById(teacherId));
    }

    [Fact]
    public async Task AddTeacher_RepositoryThrowsException_ShouldNotCallSaveChanges()
    {
        // Arrange
        var teacherDto = new TeacherDto { Id = 1 };
        
        _mockTeacherRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Teacher>()))
            .ThrowsAsync(new Exception("Ошибка добавления"));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _teacherServices.AddTeacher(teacherDto));
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Never);
    }

    // Тест для проверки, что методы расширения вызываются корректно
    [Fact]
    public async Task FetchTeachersFromBackendAsync_ShouldCallToTeachersDtoExtension()
    {
        // Arrange
        var teachers = new List<Teacher>
        {
            Schedule.CreateTeacher(1, "Иван", "Иванов", "Иванович", [], [])
        };
        
        _mockTeacherRepository
            .Setup(repo => repo.GetTeacherListAsync())
            .ReturnsAsync(teachers);

        // Act
        var result = await _teacherServices.FetchTeachersFromBackendAsync();

        // Assert
        // Проверяем, что результат преобразован в DTO
        Assert.IsType<List<TeacherDto>>(result);
        Assert.Single(result);
    }

    // Тест для проверки фабричного метода Schedule.CreateTeacher
    [Fact]
    public async Task AddTeacher_ShouldUseScheduleCreateTeacher()
    {
        // Arrange
        var teacherDto = new TeacherDto
        {
            Id = 1,
            Name = "Тест",
            Surname = "Тестов",
            Patronymic = "Тестович",
            SchoolSubjects = [],
            StudyGroups = []
        };

        Teacher capturedTeacher = null;
        _mockTeacherRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Teacher>()))
            .Callback<Teacher>(t => capturedTeacher = t)
            .Returns(Task.CompletedTask);

        // Act
        await _teacherServices.AddTeacher(teacherDto);

        // Assert
        Assert.NotNull(capturedTeacher);
        Assert.Equal(teacherDto.Id, capturedTeacher.Id);
        Assert.Equal(teacherDto.Name, capturedTeacher.Name);
    }
}