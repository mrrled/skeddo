using Application.DtoModels;
using Application.Services;
using Domain.Models;
using Domain.IRepositories;
using Moq;
using Xunit;
using Application;
// ReSharper disable PreferConcreteValueOverDefault
// ReSharper disable NullableWarningSuppressionIsUsed
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

namespace Tests.ServicesTests;

public class ClassroomServicesTests
{
    private readonly Mock<IClassroomRepository> _classroomRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ClassroomServices _classroomServices;

    public ClassroomServicesTests()
    {
        _classroomRepositoryMock = new Mock<IClassroomRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _classroomServices = new ClassroomServices(_classroomRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task FetchClassroomsFromBackendAsync_ShouldReturnClassroomDtos()
    {
        // Arrange
        var classrooms = new List<Classroom>
        {
            new Classroom(1, "Room 101", "Math room"),
            new Classroom(2, "Room 102", "Science lab")
        };
        
        _classroomRepositoryMock
            .Setup(repo => repo.GetClassroomListAsync(1))
            .ReturnsAsync(classrooms);

        // Act
        var result = await _classroomServices.FetchClassroomsFromBackendAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Room 101", result[0].Name);
        Assert.Equal("Math room", result[0].Description);
        Assert.Equal("Room 102", result[1].Name);
        Assert.Equal("Science lab", result[1].Description);
        
        _classroomRepositoryMock.Verify(repo => repo.GetClassroomListAsync(1), Times.Once);
    }

    [Fact]
    public async Task AddClassroom_ShouldAddClassroom_WhenDtoIsValid()
    {
        // Arrange
        var classroomDto = new ClassroomDto
        {
            Id = 1,
            Name = "Room 103",
            Description = "Computer lab"
        };

        Classroom? capturedClassroom = null;
        _classroomRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Classroom>(), 1))
            .Callback<Classroom, int>((classroom, _) => capturedClassroom = classroom)
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        await _classroomServices.AddClassroom(classroomDto);

        // Assert
        _classroomRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Classroom>(), 1), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
        
        Assert.NotNull(capturedClassroom);
        Assert.Equal(1, capturedClassroom.Id);
        Assert.Equal("Room 103", capturedClassroom.Name);
        Assert.Equal("Computer lab", capturedClassroom.Description);
    }

    [Fact]
    public async Task AddClassroom_ShouldThrowArgumentNullException_WhenNameIsNull()
    {
        // Arrange
        var classroomDto = new ClassroomDto
        {
            Id = 1,
            Name = null!,
            Description = "Test"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _classroomServices.AddClassroom(classroomDto));
    }

    [Fact]
    public async Task EditClassroom_ShouldUpdateClassroom_WhenChangesExist()
    {
        // Arrange
        var classroomDto = new ClassroomDto
        {
            Id = 1,
            Name = "Updated Room",
            Description = "Updated Description"
        };

        var existingClassroom = new Classroom(1, "Old Room", "Old Description");
        
        _classroomRepositoryMock
            .Setup(repo => repo.GetClassroomByIdAsync(1))
            .ReturnsAsync(existingClassroom);

        _classroomRepositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Classroom>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        await _classroomServices.EditClassroom(classroomDto);

        // Assert
        _classroomRepositoryMock.Verify(repo => repo.GetClassroomByIdAsync(1), Times.Once);
        _classroomRepositoryMock.Verify(repo => repo.UpdateAsync(existingClassroom), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
        
        Assert.Equal("Updated Room", existingClassroom.Name);
        Assert.Equal("Updated Description", existingClassroom.Description);
    }

    [Fact]
    public async Task EditClassroom_ShouldNotUpdate_WhenNoChanges()
    {
        // Arrange
        var classroomDto = new ClassroomDto
        {
            Id = 1,
            Name = "Same Room",
            Description = "Same Description"
        };

        var existingClassroom = new Classroom(1, "Same Room", "Same Description");
        
        _classroomRepositoryMock
            .Setup(repo => repo.GetClassroomByIdAsync(1))
            .ReturnsAsync(existingClassroom);

        _classroomRepositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Classroom>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        await _classroomServices.EditClassroom(classroomDto);

        // Assert
        _classroomRepositoryMock.Verify(repo => repo.GetClassroomByIdAsync(1), Times.Once);
        _classroomRepositoryMock.Verify(repo => repo.UpdateAsync(existingClassroom), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
        
        // Verify that properties remain unchanged
        Assert.Equal("Same Room", existingClassroom.Name);
        Assert.Equal("Same Description", existingClassroom.Description);
    }

    [Fact]
    public async Task EditClassroom_ShouldThrowArgumentException_WhenClassroomNotFound()
    {
        // Arrange
        var classroomDto = new ClassroomDto { Id = 999 };
        
        _classroomRepositoryMock
            .Setup(repo => repo.GetClassroomByIdAsync(999))
            .ReturnsAsync((Classroom?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _classroomServices.EditClassroom(classroomDto));
        
        Assert.Contains("Classroom with id 999 not found", exception.Message);
        _classroomRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Classroom>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public async Task EditClassroom_ShouldUpdateOnlyName_WhenDescriptionIsNull()
    {
        // Arrange
        var classroomDto = new ClassroomDto
        {
            Id = 1,
            Name = "New Name",
            Description = null
        };

        var existingClassroom = new Classroom(1, "Old Name", "Old Description");
        
        _classroomRepositoryMock
            .Setup(repo => repo.GetClassroomByIdAsync(1))
            .ReturnsAsync(existingClassroom);

        // Act
        await _classroomServices.EditClassroom(classroomDto);

        // Assert
        Assert.Equal("New Name", existingClassroom.Name);
        Assert.Null(existingClassroom.Description); // Should be set to null
    }

    [Fact]
    public async Task EditClassroom_ShouldUpdateOnlyDescription_WhenNameIsSame()
    {
        // Arrange
        var classroomDto = new ClassroomDto
        {
            Id = 1,
            Name = "Same Name",
            Description = "New Description"
        };

        var existingClassroom = new Classroom(1, "Same Name", "Old Description");
        
        _classroomRepositoryMock
            .Setup(repo => repo.GetClassroomByIdAsync(1))
            .ReturnsAsync(existingClassroom);

        // Act
        await _classroomServices.EditClassroom(classroomDto);

        // Assert
        Assert.Equal("Same Name", existingClassroom.Name); // Should remain same
        Assert.Equal("New Description", existingClassroom.Description);
    }

    [Fact]
    public async Task DeleteClassroom_ShouldDeleteClassroom_WhenClassroomExists()
    {
        // Arrange
        var classroomDto = new ClassroomDto { Id = 1 };
        var existingClassroom = new Classroom(1, "Room 101");
        
        _classroomRepositoryMock
            .Setup(repo => repo.GetClassroomByIdAsync(1))
            .ReturnsAsync(existingClassroom);

        _classroomRepositoryMock
            .Setup(repo => repo.Delete(existingClassroom))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        await _classroomServices.DeleteClassroom(classroomDto);

        // Assert
        _classroomRepositoryMock.Verify(repo => repo.GetClassroomByIdAsync(1), Times.Once);
        _classroomRepositoryMock.Verify(repo => repo.Delete(existingClassroom), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteClassroom_ShouldThrowArgumentException_WhenClassroomNotFound()
    {
        // Arrange
        var classroomDto = new ClassroomDto { Id = 999 };
        
        _classroomRepositoryMock
            .Setup(repo => repo.GetClassroomByIdAsync(999))
            .ReturnsAsync((Classroom?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _classroomServices.DeleteClassroom(classroomDto));
        
        Assert.Contains("Classroom with id 999 not found", exception.Message);
        _classroomRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Classroom>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public async Task FetchClassroomsFromBackendAsync_ShouldReturnEmptyList_WhenNoClassrooms()
    {
        // Arrange
        var emptyList = new List<Classroom>();
        
        _classroomRepositoryMock
            .Setup(repo => repo.GetClassroomListAsync(1))
            .ReturnsAsync(emptyList);

        // Act
        var result = await _classroomServices.FetchClassroomsFromBackendAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _classroomRepositoryMock.Verify(repo => repo.GetClassroomListAsync(1), Times.Once);
    }

    [Fact]
    public async Task AddClassroom_ShouldHandleNullDescription()
    {
        // Arrange
        var classroomDto = new ClassroomDto
        {
            Id = 1,
            Name = "Room 104",
            Description = null
        };

        Classroom? capturedClassroom = null;
        _classroomRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Classroom>(), 1))
            .Callback<Classroom, int>((classroom, _) => capturedClassroom = classroom)
            .Returns(Task.CompletedTask);

        // Act
        await _classroomServices.AddClassroom(classroomDto);

        // Assert
        Assert.NotNull(capturedClassroom);
        Assert.Equal("Room 104", capturedClassroom.Name);
        Assert.Null(capturedClassroom.Description);
    }

    [Fact]
    public async Task EditClassroom_ShouldThrowArgumentNullException_WhenSettingNullName()
    {
        // Arrange
        var classroomDto = new ClassroomDto
        {
            Id = 1,
            Name = null!, // This will cause SetName to throw
            Description = "Test"
        };

        var existingClassroom = new Classroom(1, "Room 101", "Description");
        
        _classroomRepositoryMock
            .Setup(repo => repo.GetClassroomByIdAsync(1))
            .ReturnsAsync(existingClassroom);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _classroomServices.EditClassroom(classroomDto));
    }
}