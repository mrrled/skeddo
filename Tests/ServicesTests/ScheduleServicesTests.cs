using Xunit;
using Moq;
using Application.DtoModels;
using Application.Services;
using Domain.Models;
using Domain.IRepositories;
using Application;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Tests.ServicesTests;

public class ScheduleServicesTests
{
    private readonly Mock<IScheduleRepository> _mockScheduleRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly ScheduleServices _scheduleServices;

    public ScheduleServicesTests()
    {
        _mockScheduleRepository = new Mock<IScheduleRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _scheduleServices = new ScheduleServices(_mockScheduleRepository.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task FetchSchedulesFromBackendAsync_ShouldReturnSchedulesDto()
    {
        // Arrange
        var schedules = new List<Schedule>
        {
            new Schedule(1, "Schedule 1", []),
            new Schedule(2, "Schedule 2", [])
        };

        _mockScheduleRepository
            .Setup(repo => repo.GetScheduleListAsync())
            .ReturnsAsync(schedules);

        // Act
        var result = await _scheduleServices.FetchSchedulesFromBackendAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("Schedule 1", result[0].Name);
        Assert.Equal(2, result[1].Id);
        Assert.Equal("Schedule 2", result[1].Name);
    }

    [Fact]
    public async Task AddSchedule_ShouldAddScheduleAndSaveChanges()
    {
        // Arrange
        var scheduleDto = new ScheduleDto { Id = 1, Name = "New Schedule" };

        _mockScheduleRepository
            .Setup(repo => repo.AddAsync(It.Is<Schedule>(s => 
                s.Id == scheduleDto.Id && s.Name == scheduleDto.Name)))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        await _scheduleServices.AddSchedule(scheduleDto);

        // Assert
        _mockScheduleRepository.Verify(
            repo => repo.AddAsync(It.Is<Schedule>(s => 
                s.Id == scheduleDto.Id && s.Name == scheduleDto.Name)),
            Times.Once);

        _mockUnitOfWork.Verify(
            uow => uow.SaveChangesAsync(default),
            Times.Once);
    }

    [Fact]
    public async Task EditSchedule_ShouldUpdateScheduleAndSaveChanges()
    {
        // Arrange
        var oldScheduleDto = new ScheduleDto { Id = 1, Name = "Old Schedule" };
        var newScheduleDto = new ScheduleDto { Id = 1, Name = "Updated Schedule" };

        _mockScheduleRepository
            .Setup(repo => repo.UpdateAsync(
                It.Is<Schedule>(old => old.Id == oldScheduleDto.Id && old.Name == oldScheduleDto.Name),
                It.Is<Schedule>(n => n.Id == newScheduleDto.Id && n.Name == newScheduleDto.Name)))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        await _scheduleServices.EditSchedule(oldScheduleDto, newScheduleDto);

        // Assert
        _mockScheduleRepository.Verify(
            repo => repo.UpdateAsync(
                It.Is<Schedule>(old => old.Id == oldScheduleDto.Id && old.Name == oldScheduleDto.Name),
                It.Is<Schedule>(n => n.Id == newScheduleDto.Id && n.Name == newScheduleDto.Name)),
            Times.Once);

        _mockUnitOfWork.Verify(
            uow => uow.SaveChangesAsync(default),
            Times.Once);
    }

    [Fact]
    public async Task DeleteSchedule_ShouldDeleteScheduleAndSaveChanges()
    {
        // Arrange
        var scheduleDto = new ScheduleDto { Id = 1, Name = "Schedule to Delete" };

        _mockScheduleRepository
            .Setup(repo => repo.Delete(It.Is<Schedule>(s => 
                s.Id == scheduleDto.Id && s.Name == scheduleDto.Name)))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        await _scheduleServices.DeleteSchedule(scheduleDto);

        // Assert
        _mockScheduleRepository.Verify(
            repo => repo.Delete(It.Is<Schedule>(s => 
                s.Id == scheduleDto.Id && s.Name == scheduleDto.Name)),
            Times.Once);

        _mockUnitOfWork.Verify(
            uow => uow.SaveChangesAsync(default),
            Times.Once);
    }

    [Fact]
    public async Task FetchSchedulesFromBackendAsync_ShouldReturnEmptyList_WhenNoSchedulesExist()
    {
        // Arrange
        var emptySchedules = new List<Schedule>();
        
        _mockScheduleRepository
            .Setup(repo => repo.GetScheduleListAsync())
            .ReturnsAsync(emptySchedules);

        // Act
        var result = await _scheduleServices.FetchSchedulesFromBackendAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddSchedule_ShouldThrowException_WhenRepositoryFails()
    {
        // Arrange
        var scheduleDto = new ScheduleDto { Id = 1, Name = "New Schedule" };

        _mockScheduleRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Schedule>()))
            .ThrowsAsync(new System.Exception("Repository error"));

        // Act & Assert
        await Assert.ThrowsAsync<System.Exception>(() => 
            _scheduleServices.AddSchedule(scheduleDto));
    }

    [Fact]
    public async Task EditSchedule_ShouldMapCorrectly_WhenIdsDiffer()
    {
        // Arrange
        // Note: Based on the provided code, EditSchedule uses oldScheduleDto.Id for both objects
        var oldScheduleDto = new ScheduleDto { Id = 1, Name = "Old Name" };
        var newScheduleDto = new ScheduleDto { Id = 2, Name = "New Name" }; // Different ID!

        // The service should create both Schedule objects with oldScheduleDto.Id
        _mockScheduleRepository
            .Setup(repo => repo.UpdateAsync(
                It.Is<Schedule>(old => old.Id == oldScheduleDto.Id && old.Name == oldScheduleDto.Name),
                It.Is<Schedule>(n => n.Id == oldScheduleDto.Id && n.Name == newScheduleDto.Name)))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        await _scheduleServices.EditSchedule(oldScheduleDto, newScheduleDto);

        // Assert
        _mockScheduleRepository.Verify(
            repo => repo.UpdateAsync(
                It.Is<Schedule>(old => old.Id == oldScheduleDto.Id && old.Name == oldScheduleDto.Name),
                It.Is<Schedule>(n => n.Id == oldScheduleDto.Id && n.Name == newScheduleDto.Name)),
            Times.Once);
    }

    [Fact]
    public async Task Constructor_ShouldInitializeDependencies()
    {
        // Arrange & Act
        var services = new ScheduleServices(_mockScheduleRepository.Object, _mockUnitOfWork.Object);

        // Assert
        Assert.NotNull(services);
    }

    [Fact]
    public async Task ScheduleDto_ShouldCreateScheduleWithEmptyItemsList()
    {
        // This test verifies the behavior described in the service methods
        // Arrange
        var scheduleDto = new ScheduleDto { Id = 1, Name = "Test Schedule" };
        
        // Act
        var schedule = new Schedule(scheduleDto.Id, scheduleDto.Name, []);

        // Assert
        Assert.Equal(scheduleDto.Id, schedule.Id);
        Assert.Equal(scheduleDto.Name, schedule.Name);
        Assert.NotNull(schedule.Lessons);
        Assert.Empty(schedule.Lessons);
    }
}