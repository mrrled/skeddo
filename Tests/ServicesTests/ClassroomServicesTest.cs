using Application.DtoModels;
using Application.Services;
using Domain.Models;
using Domain.IRepositories;
using Moq;
using Xunit;
using Application;
// ReSharper disable PreferConcreteValueOverDefault

namespace Tests.ServicesTests;

public class ClassroomServicesTests
{
    private readonly Mock<IClassroomRepository> _mockClassroomRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly ClassroomServices _classroomServices;

    public ClassroomServicesTests()
    {
        _mockClassroomRepository = new Mock<IClassroomRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _classroomServices = new ClassroomServices(
            _mockClassroomRepository.Object, 
            _mockUnitOfWork.Object
        );
    }

    [Fact]
    public async Task FetchClassroomsFromBackendAsync_ShouldReturnClassroomList()
    {
        // Arrange
        var classrooms = new List<Classroom>
        {
            Schedule.CreateClassroom("608", "DM"),
            Schedule.CreateClassroom("532", "Matan")
        };

        _mockClassroomRepository
            .Setup(repo => repo.GetClassroomListAsync())
            .ReturnsAsync(classrooms);

        // Act
        var result = await _classroomServices.FetchClassroomsFromBackendAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("608", result[0].Name);
        Assert.Equal("DM", result[0].Description);
        _mockClassroomRepository.Verify(repo => repo.GetClassroomListAsync(), Times.Once);
    }

    [Fact]
    public async Task AddClassroom_ShouldAddClassroomAndSaveChanges()
    {
        // Arrange
        var classroomDto = new ClassroomDto { Name = "700", Description = "T*rv*r" };
    
        _mockClassroomRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Classroom>()))
            .Returns(Task.CompletedTask);
    
        _mockUnitOfWork
            .Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);
    
        // Act
        await _classroomServices.AddClassroom(classroomDto);
    
        // Assert
        _mockClassroomRepository.Verify(repo => repo.AddAsync(
            It.Is<Classroom>(c => c.Name == "700" && c.Description == "T*rv*r")), 
            Times.Once
        );
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }
    
    [Fact]
    public async Task EditClassroom_ShouldUpdateClassroomAndSaveChanges()
    {
        // Arrange
        var oldClassroomDto = new ClassroomDto { Name = "A101", Description = "Old Description" };
        var newClassroomDto = new ClassroomDto { Name = "A101", Description = "New Description" };
    
        _mockClassroomRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<Classroom>(), It.IsAny<Classroom>()))
            .Returns(Task.CompletedTask);
    
        _mockUnitOfWork
            .Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);
    
        // Act
        await _classroomServices.EditClassroom(oldClassroomDto, newClassroomDto);
    
        // Assert
        _mockClassroomRepository.Verify(repo => repo.UpdateAsync(
            It.Is<Classroom>(old => old.Name == "A101" && old.Description == "Old Description"),
            It.Is<Classroom>(newClass => newClass.Name == "A101" && newClass.Description == "New Description")),
            Times.Once
        );
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }
    
    [Fact]
    public async Task DeleteClassroom_ShouldDeleteClassroomAndSaveChanges()
    {
        // Arrange
        var classroomDto = new ClassroomDto { Name = "A101", Description = "Lecture Hall" };
    
        _mockClassroomRepository
            .Setup(repo => repo.Delete(It.IsAny<Classroom>()))
            .Returns(Task.CompletedTask);
    
        _mockUnitOfWork
            .Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);
    
        // Act
        await _classroomServices.DeleteClassroom(classroomDto);
    
        // Assert
        _mockClassroomRepository.Verify(repo => repo.Delete(
            It.Is<Classroom>(c => c.Name == "A101" && c.Description == "Lecture Hall")),
            Times.Once
        );
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }
    
    [Fact]
    public async Task FetchClassroomsFromBackendAsync_WhenRepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        _mockClassroomRepository
            .Setup(repo => repo.GetClassroomListAsync())
            .ThrowsAsync(new NullReferenceException("No schedule group found"));
    
        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => 
            _classroomServices.FetchClassroomsFromBackendAsync()
        );
    }
    
    [Fact]
    public async Task AddClassroom_WhenRepositoryThrowsException_ShouldNotSaveChanges()
    {
        // Arrange
        var classroomDto = new ClassroomDto { Name = "C303", Description = "Science Lab" };
    
        _mockClassroomRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Classroom>()))
            .ThrowsAsync(new NullReferenceException("No schedule group found"));
    
        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => 
            _classroomServices.AddClassroom(classroomDto)
        );
    
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Never);
    }
    
    [Theory]
    [InlineData("", "Description")] // Empty name
    //[InlineData(null, "Description")] // Null name
    [InlineData("A101", "")] // Empty description
    //[InlineData("A101", null)] // Null description
    // TODO: Сделать тест на нулевое имя
    // TODO: Сделать тест на нулевое описание
    public async Task AddClassroom_WithInvalidData_ShouldStillCallRepository(string name, string description)
    {
        // Arrange
        var classroomDto = new ClassroomDto { Name = name, Description = description };
    
        _mockClassroomRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Classroom>()))
            .Returns(Task.CompletedTask);
    
        _mockUnitOfWork
            .Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);
    
        // Act
        await _classroomServices.AddClassroom(classroomDto);
    
        // Assert
        _mockClassroomRepository.Verify(repo => repo.AddAsync(It.IsAny<Classroom>()), Times.Once);
    }
}