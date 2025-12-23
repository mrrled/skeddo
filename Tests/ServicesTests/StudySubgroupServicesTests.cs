using Application.DtoModels;
using Application.Services;
using Domain.IRepositories;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Application;

namespace Tests.ServicesTests;

public class StudySubgroupServicesTests
{
    private readonly Mock<IStudyGroupRepository> _studyGroupRepositoryMock;
    private readonly Mock<IStudySubgroupRepository> _studySubgroupRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<StudySubgroupServices>> _loggerMock;
    private readonly StudySubgroupServices _services;

    public StudySubgroupServicesTests()
    {
        _studyGroupRepositoryMock = new Mock<IStudyGroupRepository>();
        _studySubgroupRepositoryMock = new Mock<IStudySubgroupRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<StudySubgroupServices>>();
        
        _services = new StudySubgroupServices(
            _studyGroupRepositoryMock.Object,
            _studySubgroupRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    #region AddStudySubgroup Tests

    [Fact]
    public async Task AddStudySubgroup_WhenStudyGroupNotFound_ReturnsFailure()
    {
        // Arrange
        var studySubgroupDto = new StudySubgroupDto
        {
            Name = "Подгруппа 1",
            StudyGroup = new StudyGroupDto { Id = Guid.NewGuid() }
        };

        _studyGroupRepositoryMock
            .Setup(r => r.GetStudyGroupByIdAsync(studySubgroupDto.StudyGroup.Id))
            .ReturnsAsync((StudyGroup)null);

        // Act
        var result = await _services.AddStudySubgroup(studySubgroupDto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Учебная группа не найдена.", result.Error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task AddStudySubgroup_WhenNameIsInvalid_ReturnsFailure(string invalidName)
    {
        // Arrange
        var studyGroup = StudyGroup.CreateStudyGroup(Guid.NewGuid(), Guid.NewGuid(), "Группа 1").Value;
        var studySubgroupDto = new StudySubgroupDto
        {
            Name = invalidName,
            StudyGroup = new StudyGroupDto { Id = studyGroup.Id }
        };

        _studyGroupRepositoryMock
            .Setup(r => r.GetStudyGroupByIdAsync(studyGroup.Id))
            .ReturnsAsync(studyGroup);

        // Act
        var result = await _services.AddStudySubgroup(studySubgroupDto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Название учебной подгруппы не может быть пустым", result.Error);
    }

    [Fact]
    public async Task AddStudySubgroup_WhenRepositoryThrowsException_ReturnsFailure()
    {
        // Arrange
        var studyGroup = StudyGroup.CreateStudyGroup(Guid.NewGuid(), Guid.NewGuid(), "Группа 1").Value;
        var studySubgroupDto = new StudySubgroupDto
        {
            Name = "Подгруппа 1",
            StudyGroup = new StudyGroupDto { Id = studyGroup.Id }
        };

        var studySubgroup = StudySubgroup.CreateStudySubgroup(studyGroup, studySubgroupDto.Name).Value;

        _studyGroupRepositoryMock
            .Setup(r => r.GetStudyGroupByIdAsync(studyGroup.Id))
            .ReturnsAsync(studyGroup);

        _studySubgroupRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<StudySubgroup>(), studyGroup.Id))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act
        var result = await _services.AddStudySubgroup(studySubgroupDto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Запись не найдена в базе данных.", result.Error);
    }

    [Fact]
    public async Task AddStudySubgroup_WhenValidData_ReturnsSuccess()
    {
        // Arrange
        var studyGroup = StudyGroup.CreateStudyGroup(Guid.NewGuid(), Guid.NewGuid(), "Группа 1").Value;
        var studySubgroupDto = new StudySubgroupDto
        {
            Name = "Подгруппа 1",
            StudyGroup = new StudyGroupDto { Id = studyGroup.Id }
        };

        var studySubgroup = StudySubgroup.CreateStudySubgroup(studyGroup, studySubgroupDto.Name).Value;

        _studyGroupRepositoryMock
            .Setup(r => r.GetStudyGroupByIdAsync(studyGroup.Id))
            .ReturnsAsync(studyGroup);

        _studySubgroupRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<StudySubgroup>(), studyGroup.Id))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _services.AddStudySubgroup(studySubgroupDto);

        // Assert
        Assert.True(result.IsSuccess);
        _studySubgroupRepositoryMock.Verify(
            r => r.AddAsync(It.Is<StudySubgroup>(s => 
                s.Name == studySubgroupDto.Name && 
                s.StudyGroup.Id == studyGroup.Id), 
                studyGroup.Id),
            Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region EditStudySubgroup Tests

    [Fact]
    public async Task EditStudySubgroup_WhenStudyGroupIdsDontMatch_ReturnsFailure()
    {
        // Arrange
        var oldDto = new StudySubgroupDto
        {
            Name = "Подгруппа 1",
            StudyGroup = new StudyGroupDto { Id = Guid.NewGuid() }
        };

        var newDto = new StudySubgroupDto
        {
            Name = "Подгруппа 2",
            StudyGroup = new StudyGroupDto { Id = Guid.NewGuid() }
        };

        // Act
        var result = await _services.EditStudySubgroup(oldDto, newDto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Смена учебной группы запрещена.", result.Error);
    }

    [Fact]
    public async Task EditStudySubgroup_WhenStudyGroupNotFound_ReturnsFailure()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var oldDto = new StudySubgroupDto
        {
            Name = "Подгруппа 1",
            StudyGroup = new StudyGroupDto { Id = groupId }
        };

        var newDto = new StudySubgroupDto
        {
            Name = "Подгруппа 2",
            StudyGroup = new StudyGroupDto { Id = groupId }
        };

        _studyGroupRepositoryMock
            .Setup(r => r.GetStudyGroupByIdAsync(groupId))
            .ReturnsAsync((StudyGroup)null);

        // Act
        var result = await _services.EditStudySubgroup(oldDto, newDto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Учебная группа не найдена.", result.Error);
    }

    [Theory]
    [InlineData(null, "Новое имя")]
    [InlineData("Старое имя", "")]
    [InlineData("   ", "   ")]
    public async Task EditStudySubgroup_WhenNameIsInvalid_ReturnsFailure(string oldName, string newName)
    {
        // Arrange
        var studyGroup = StudyGroup.CreateStudyGroup(Guid.NewGuid(), Guid.NewGuid(), "Группа 1").Value;
        var oldDto = new StudySubgroupDto
        {
            Name = oldName,
            StudyGroup = new StudyGroupDto { Id = studyGroup.Id }
        };

        var newDto = new StudySubgroupDto
        {
            Name = newName,
            StudyGroup = new StudyGroupDto { Id = studyGroup.Id }
        };

        _studyGroupRepositoryMock
            .Setup(r => r.GetStudyGroupByIdAsync(studyGroup.Id))
            .ReturnsAsync(studyGroup);

        // Act
        var result = await _services.EditStudySubgroup(oldDto, newDto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Название учебной подгруппы не может быть пустым", result.Error);
    }

    [Fact]
    public async Task EditStudySubgroup_WhenUpdateFails_ReturnsFailure()
    {
        // Arrange
        var studyGroup = StudyGroup.CreateStudyGroup(Guid.NewGuid(), Guid.NewGuid(), "Группа 1").Value;
        var oldDto = new StudySubgroupDto
        {
            Name = "Подгруппа 1",
            StudyGroup = new StudyGroupDto { Id = studyGroup.Id }
        };

        var newDto = new StudySubgroupDto
        {
            Name = "Подгруппа 2",
            StudyGroup = new StudyGroupDto { Id = studyGroup.Id }
        };

        _studyGroupRepositoryMock
            .Setup(r => r.GetStudyGroupByIdAsync(studyGroup.Id))
            .ReturnsAsync(studyGroup);

        _studySubgroupRepositoryMock
            .Setup(r => r.UpdateAsync(
                It.IsAny<StudySubgroup>(), 
                It.IsAny<StudySubgroup>(), 
                studyGroup.Id))
            .ThrowsAsync(new InvalidOperationException("Update error"));

        // Act
        var result = await _services.EditStudySubgroup(oldDto, newDto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Запись не найдена в базе данных.", result.Error);
    }

    [Fact]
    public async Task EditStudySubgroup_WhenValidData_ReturnsSuccess()
    {
        // Arrange
        var studyGroup = StudyGroup.CreateStudyGroup(Guid.NewGuid(), Guid.NewGuid(), "Группа 1").Value;
        var oldDto = new StudySubgroupDto
        {
            Name = "Подгруппа 1",
            StudyGroup = new StudyGroupDto { Id = studyGroup.Id }
        };

        var newDto = new StudySubgroupDto
        {
            Name = "Подгруппа 2",
            StudyGroup = new StudyGroupDto { Id = studyGroup.Id }
        };

        var oldStudySubgroup = StudySubgroup.CreateStudySubgroup(studyGroup, oldDto.Name).Value;
        var newStudySubgroup = StudySubgroup.CreateStudySubgroup(studyGroup, newDto.Name).Value;

        _studyGroupRepositoryMock
            .Setup(r => r.GetStudyGroupByIdAsync(studyGroup.Id))
            .ReturnsAsync(studyGroup);

        _studySubgroupRepositoryMock
            .Setup(r => r.UpdateAsync(
                It.IsAny<StudySubgroup>(), 
                It.IsAny<StudySubgroup>(), 
                studyGroup.Id))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _services.EditStudySubgroup(oldDto, newDto);

        // Assert
        Assert.True(result.IsSuccess);
        _studySubgroupRepositoryMock.Verify(
            r => r.UpdateAsync(
                It.Is<StudySubgroup>(s => s.Name == oldDto.Name),
                It.Is<StudySubgroup>(s => s.Name == newDto.Name),
                studyGroup.Id),
            Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region DeleteStudySubgroup Tests

    [Fact]
    public async Task DeleteStudySubgroup_WhenStudyGroupNotFound_ReturnsFailure()
    {
        // Arrange
        var studySubgroupDto = new StudySubgroupDto
        {
            Name = "Подгруппа 1",
            StudyGroup = new StudyGroupDto { Id = Guid.NewGuid() }
        };

        _studyGroupRepositoryMock
            .Setup(r => r.GetStudyGroupByIdAsync(studySubgroupDto.StudyGroup.Id))
            .ReturnsAsync((StudyGroup)null);

        // Act
        var result = await _services.DeleteStudySubgroup(studySubgroupDto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Учебная группа не найдена.", result.Error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task DeleteStudySubgroup_WhenNameIsInvalid_ReturnsFailure(string invalidName)
    {
        // Arrange
        var studyGroup = StudyGroup.CreateStudyGroup(Guid.NewGuid(), Guid.NewGuid(), "Группа 1").Value;
        var studySubgroupDto = new StudySubgroupDto
        {
            Name = invalidName,
            StudyGroup = new StudyGroupDto { Id = studyGroup.Id }
        };

        _studyGroupRepositoryMock
            .Setup(r => r.GetStudyGroupByIdAsync(studyGroup.Id))
            .ReturnsAsync(studyGroup);

        // Act
        var result = await _services.DeleteStudySubgroup(studySubgroupDto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Название учебной подгруппы не может быть пустым", result.Error);
    }

    [Fact]
    public async Task DeleteStudySubgroup_WhenValidData_ReturnsSuccess()
    {
        // Arrange
        var studyGroup = StudyGroup.CreateStudyGroup(Guid.NewGuid(), Guid.NewGuid(), "Группа 1").Value;
        var studySubgroupDto = new StudySubgroupDto
        {
            Name = "Подгруппа 1",
            StudyGroup = new StudyGroupDto { Id = studyGroup.Id }
        };

        var studySubgroup = StudySubgroup.CreateStudySubgroup(studyGroup, studySubgroupDto.Name).Value;

        _studyGroupRepositoryMock
            .Setup(r => r.GetStudyGroupByIdAsync(studyGroup.Id))
            .ReturnsAsync(studyGroup);

        _studySubgroupRepositoryMock
            .Setup(r => r.Delete(It.IsAny<StudySubgroup>(), studyGroup.Id))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _services.DeleteStudySubgroup(studySubgroupDto);

        // Assert
        Assert.True(result.IsSuccess);
        _studySubgroupRepositoryMock.Verify(
            r => r.Delete(
                It.Is<StudySubgroup>(s => 
                    s.Name == studySubgroupDto.Name && 
                    s.StudyGroup.Id == studyGroup.Id), 
                studyGroup.Id),
            Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Edge Cases Tests

    [Fact]
    public async Task EditStudySubgroup_WhenNamesAreSame_StillProcessesSuccessfully()
    {
        // Arrange
        var studyGroup = StudyGroup.CreateStudyGroup(Guid.NewGuid(), Guid.NewGuid(), "Группа 1").Value;
        var name = "Подгруппа 1";
        var oldDto = new StudySubgroupDto
        {
            Name = name,
            StudyGroup = new StudyGroupDto { Id = studyGroup.Id }
        };

        var newDto = new StudySubgroupDto
        {
            Name = name, // То же самое имя
            StudyGroup = new StudyGroupDto { Id = studyGroup.Id }
        };

        _studyGroupRepositoryMock
            .Setup(r => r.GetStudyGroupByIdAsync(studyGroup.Id))
            .ReturnsAsync(studyGroup);

        _studySubgroupRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<StudySubgroup>(), It.IsAny<StudySubgroup>(), studyGroup.Id))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _services.EditStudySubgroup(oldDto, newDto);

        // Assert
        Assert.True(result.IsSuccess);
        // Хотя имена одинаковые, операция все равно должна пройти
    }

    [Fact]
    public async Task AddStudySubgroup_WhenStudySubgroupAlreadyExists_RepositoryHandlesIt()
    {
        // Arrange
        var studyGroup = StudyGroup.CreateStudyGroup(Guid.NewGuid(), Guid.NewGuid(), "Группа 1").Value;
        var studySubgroupDto = new StudySubgroupDto
        {
            Name = "Подгруппа 1",
            StudyGroup = new StudyGroupDto { Id = studyGroup.Id }
        };

        _studyGroupRepositoryMock
            .Setup(r => r.GetStudyGroupByIdAsync(studyGroup.Id))
            .ReturnsAsync(studyGroup);

        _studySubgroupRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<StudySubgroup>(), studyGroup.Id))
            .ThrowsAsync(new InvalidOperationException("Подгруппа уже существует"));

        // Act
        var result = await _services.AddStudySubgroup(studySubgroupDto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Запись не найдена в базе данных.", result.Error);
    }

    [Fact]
    public async Task EditStudySubgroup_WhenOldStudySubgroupNotFound_ReturnsFailure()
    {
        // Arrange
        var studyGroup = StudyGroup.CreateStudyGroup(Guid.NewGuid(), Guid.NewGuid(), "Группа 1").Value;
        var oldDto = new StudySubgroupDto
        {
            Name = "Несуществующая подгруппа",
            StudyGroup = new StudyGroupDto { Id = studyGroup.Id }
        };

        var newDto = new StudySubgroupDto
        {
            Name = "Подгруппа 2",
            StudyGroup = new StudyGroupDto { Id = studyGroup.Id }
        };

        _studyGroupRepositoryMock
            .Setup(r => r.GetStudyGroupByIdAsync(studyGroup.Id))
            .ReturnsAsync(studyGroup);

        _studySubgroupRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<StudySubgroup>(), It.IsAny<StudySubgroup>(), studyGroup.Id))
            .ThrowsAsync(new InvalidOperationException("Подгруппа не найдена"));

        // Act
        var result = await _services.EditStudySubgroup(oldDto, newDto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Запись не найдена в базе данных.", result.Error);
    }

    #endregion
}