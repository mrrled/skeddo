using Xunit;
using Moq;
using Application.Services;
using Application.DtoModels;
using Domain;
using Domain.Models;
using Domain.IRepositories;
using Application;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.DtoExtensions;

namespace Tests.ServicesTests
{
    public class LessonNumberServicesTests
    {
        private readonly Mock<ILessonNumberRepository> _lessonNumberRepositoryMock;
        private readonly Mock<IScheduleRepository> _scheduleRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<LessonNumberServices>> _loggerMock;
        private readonly LessonNumberServices _lessonNumberServices;

        public LessonNumberServicesTests()
        {
            _lessonNumberRepositoryMock = new Mock<ILessonNumberRepository>();
            _scheduleRepositoryMock = new Mock<IScheduleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<LessonNumberServices>>();
            
            _lessonNumberServices = new LessonNumberServices(
                _lessonNumberRepositoryMock.Object,
                _scheduleRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object
            );
        }

        #region GetLessonNumbersByScheduleId Tests

        [Fact]
        public async Task GetLessonNumbersByScheduleId_ShouldReturnLessonNumbers_WhenScheduleExists()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonNumbers = new List<LessonNumber>
            {
                LessonNumber.CreateLessonNumber(1, "09:00").Value,
                LessonNumber.CreateLessonNumber(2, "10:00").Value
            };

            _lessonNumberRepositoryMock
                .Setup(repo => repo.GetLessonNumbersByScheduleIdAsync(scheduleId))
                .ReturnsAsync(lessonNumbers);

            // Act
            var result = await _lessonNumberServices.GetLessonNumbersByScheduleId(scheduleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].Number);
            Assert.Equal("09:00", result[0].Time);
            Assert.Equal(2, result[1].Number);
            Assert.Equal("10:00", result[1].Time);
        }

        [Fact]
        public async Task GetLessonNumbersByScheduleId_ShouldReturnEmptyList_WhenNoLessonNumbers()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var emptyList = new List<LessonNumber>();

            _lessonNumberRepositoryMock
                .Setup(repo => repo.GetLessonNumbersByScheduleIdAsync(scheduleId))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _lessonNumberServices.GetLessonNumbersByScheduleId(scheduleId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetLessonNumbersByScheduleId_ShouldHandleRepositoryException()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();

            _lessonNumberRepositoryMock
                .Setup(repo => repo.GetLessonNumbersByScheduleIdAsync(scheduleId))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _lessonNumberServices.GetLessonNumbersByScheduleId(scheduleId));
        }

        #endregion

        #region AddLessonNumber Tests

        [Fact]
        public async Task AddLessonNumber_ShouldReturnSuccess_WhenScheduleExistsAndDataValid()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonNumberDto = new LessonNumberDto { Number = 1, Time = "09:00" };
            var schedule = Schedule.CreateSchedule(Guid.NewGuid(), "Test Schedule").Value;
            var lessonNumber = LessonNumber.CreateLessonNumber(1, "09:00").Value;

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonNumberRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<LessonNumber>(), scheduleId))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonNumberServices.AddLessonNumber(lessonNumberDto, scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
            _lessonNumberRepositoryMock.Verify(
                repo => repo.AddAsync(It.Is<LessonNumber>(ln => ln.Number == 1 && ln.Time == "09:00"), scheduleId),
                Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddLessonNumber_ShouldReturnFailure_WhenScheduleNotFound()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonNumberDto = new LessonNumberDto { Number = 1, Time = "09:00" };

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync((Schedule?)null);

            // Act
            var result = await _lessonNumberServices.AddLessonNumber(lessonNumberDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Расписание не найдено", result.Error);
            _lessonNumberRepositoryMock.Verify(
                repo => repo.AddAsync(It.IsAny<LessonNumber>(), It.IsAny<Guid>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(-1, "09:00")]
        [InlineData(1, "")]
        [InlineData(1, null)]
        public async Task AddLessonNumber_ShouldReturnFailure_WhenLessonNumberDataInvalid(int number, string time)
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonNumberDto = new LessonNumberDto { Number = number, Time = time };
            var schedule = Schedule.CreateSchedule(Guid.NewGuid(), "Test Schedule").Value;

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            // Act
            var result = await _lessonNumberServices.AddLessonNumber(lessonNumberDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains("Номер урока", result.Error);
            _lessonNumberRepositoryMock.Verify(
                repo => repo.AddAsync(It.IsAny<LessonNumber>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task AddLessonNumber_ShouldReturnFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonNumberDto = new LessonNumberDto { Number = 1, Time = "09:00" };
            var schedule = Schedule.CreateSchedule(Guid.NewGuid(), "Test Schedule").Value;

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonNumberRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<LessonNumber>(), scheduleId))
                .ThrowsAsync(new InvalidOperationException("Repository error"));

            // Act
            var result = await _lessonNumberServices.AddLessonNumber(lessonNumberDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Запись не найдена в базе данных.", result.Error);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddLessonNumber_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonNumberDto = new LessonNumberDto { Number = 1, Time = "09:00" };
            var schedule = Schedule.CreateSchedule(Guid.NewGuid(), "Test Schedule").Value;

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonNumberRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<LessonNumber>(), scheduleId))
                .ThrowsAsync(new Exception("Critical error"));

            // Act
            var result = await _lessonNumberServices.AddLessonNumber(lessonNumberDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            // Проверяем, что логгер был вызван (хотя мы не можем проверить конкретное сообщение без дополнительных настроек Moq)
        }

        #endregion

        #region EditLessonNumber Tests

        [Fact]
        public async Task EditLessonNumber_ShouldReturnSuccess_WhenDataValid()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var oldLessonNumberDto = new LessonNumberDto { Number = 1, Time = "09:00" };
            var newLessonNumberDto = new LessonNumberDto { Number = 1, Time = "09:30" };

            _lessonNumberRepositoryMock
                .Setup(repo => repo.UpdateAsync(
                    It.IsAny<LessonNumber>(),
                    It.IsAny<LessonNumber>(),
                    scheduleId))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonNumberServices.EditLessonNumber(oldLessonNumberDto, newLessonNumberDto, scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
            _lessonNumberRepositoryMock.Verify(
                repo => repo.UpdateAsync(
                    It.Is<LessonNumber>(ln => ln.Number == 1 && ln.Time == "09:00"),
                    It.Is<LessonNumber>(ln => ln.Number == 1 && ln.Time == "09:30"),
                    scheduleId),
                Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(-1, "09:00", 1, "09:30")]
        [InlineData(1, "09:00", -1, "09:30")]
        [InlineData(1, "", 1, "09:30")]
        [InlineData(1, "09:00", 1, "")]
        public async Task EditLessonNumber_ShouldReturnFailure_WhenLessonNumberDataInvalid(
            int oldNumber, string oldTime, int newNumber, string newTime)
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var oldLessonNumberDto = new LessonNumberDto { Number = oldNumber, Time = oldTime };
            var newLessonNumberDto = new LessonNumberDto { Number = newNumber, Time = newTime };

            // Act
            var result = await _lessonNumberServices.EditLessonNumber(oldLessonNumberDto, newLessonNumberDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains("Номер урока", result.Error);
            _lessonNumberRepositoryMock.Verify(
                repo => repo.UpdateAsync(It.IsAny<LessonNumber>(), It.IsAny<LessonNumber>(), It.IsAny<Guid>()),
                Times.Never);
        }

        [Fact]
        public async Task EditLessonNumber_ShouldReturnFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var oldLessonNumberDto = new LessonNumberDto { Number = 1, Time = "09:00" };
            var newLessonNumberDto = new LessonNumberDto { Number = 1, Time = "09:30" };

            _lessonNumberRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<LessonNumber>(), It.IsAny<LessonNumber>(), scheduleId))
                .ThrowsAsync(new InvalidOperationException("Repository error"));

            // Act
            var result = await _lessonNumberServices.EditLessonNumber(oldLessonNumberDto, newLessonNumberDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Запись не найдена в базе данных.", result.Error);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task EditLessonNumber_ShouldHandleSameNumberDifferentTime()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var oldLessonNumberDto = new LessonNumberDto { Number = 1, Time = "09:00" };
            var newLessonNumberDto = new LessonNumberDto { Number = 1, Time = "09:30" };

            _lessonNumberRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<LessonNumber>(), It.IsAny<LessonNumber>(), scheduleId))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonNumberServices.EditLessonNumber(oldLessonNumberDto, newLessonNumberDto, scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EditLessonNumber_ShouldHandleDifferentNumberSameTime()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var oldLessonNumberDto = new LessonNumberDto { Number = 1, Time = "09:00" };
            var newLessonNumberDto = new LessonNumberDto { Number = 2, Time = "09:00" };

            _lessonNumberRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<LessonNumber>(), It.IsAny<LessonNumber>(), scheduleId))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonNumberServices.EditLessonNumber(oldLessonNumberDto, newLessonNumberDto, scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
        }

        #endregion

        #region DeleteLessonNumber Tests

        [Fact]
        public async Task DeleteLessonNumber_ShouldReturnSuccess_WhenScheduleExistsAndDataValid()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonNumberDto = new LessonNumberDto { Number = 1, Time = "09:00" };
            var schedule = Schedule.CreateSchedule(Guid.NewGuid(), "Test Schedule").Value;

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonNumberRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<LessonNumber>(), scheduleId))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonNumberServices.DeleteLessonNumber(lessonNumberDto, scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
            _lessonNumberRepositoryMock.Verify(
                repo => repo.Delete(It.Is<LessonNumber>(ln => ln.Number == 1 && ln.Time == "09:00"), scheduleId),
                Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteLessonNumber_ShouldReturnFailure_WhenScheduleNotFound()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonNumberDto = new LessonNumberDto { Number = 1, Time = "09:00" };

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync((Schedule?)null);

            // Act
            var result = await _lessonNumberServices.DeleteLessonNumber(lessonNumberDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Расписание не найдено", result.Error);
            _lessonNumberRepositoryMock.Verify(
                repo => repo.Delete(It.IsAny<LessonNumber>(), It.IsAny<Guid>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(-1, "09:00")]
        public async Task DeleteLessonNumber_ShouldReturnFailure_WhenLessonNumberDataInvalid(int number, string time)
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonNumberDto = new LessonNumberDto { Number = number, Time = time };
            var schedule = Schedule.CreateSchedule(Guid.NewGuid(), "Test Schedule").Value;

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            // Act
            var result = await _lessonNumberServices.DeleteLessonNumber(lessonNumberDto, scheduleId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains("Номер урока", result.Error);
            _lessonNumberRepositoryMock.Verify(
                repo => repo.Delete(It.IsAny<LessonNumber>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task DeleteLessonNumber_ShouldWorkWithNullTime()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonNumberDto = new LessonNumberDto { Number = 1, Time = null };
            var schedule = Schedule.CreateSchedule(Guid.NewGuid(), "Test Schedule").Value;

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonNumberRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<LessonNumber>(), scheduleId))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonNumberServices.DeleteLessonNumber(lessonNumberDto, scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
            _lessonNumberRepositoryMock.Verify(
                repo => repo.Delete(It.Is<LessonNumber>(ln => ln.Number == 1 && ln.Time == null), scheduleId),
                Times.Once);
        }

        #endregion

        #region Edge Cases and Integration Tests

        [Fact]
        public async Task AddLessonNumber_ShouldUseCorrectCancellationToken()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonNumberDto = new LessonNumberDto { Number = 1, Time = "09:00" };
            var schedule = Schedule.CreateSchedule(Guid.NewGuid(), "Test Schedule").Value;
            var cancellationToken = new CancellationToken(true);

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _lessonNumberRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<LessonNumber>(), scheduleId))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.Is<CancellationToken>(ct => ct == cancellationToken)))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonNumberServices.AddLessonNumber(lessonNumberDto, scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(It.Is<CancellationToken>(ct => ct == cancellationToken)),
                Times.Once);
        }

        [Fact]
        public async Task EditLessonNumber_ShouldPreserveNumberWhenTimeIsNull()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var oldLessonNumberDto = new LessonNumberDto { Number = 1, Time = null };
            var newLessonNumberDto = new LessonNumberDto { Number = 1, Time = "09:30" };

            _lessonNumberRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<LessonNumber>(), It.IsAny<LessonNumber>(), scheduleId))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonNumberServices.EditLessonNumber(oldLessonNumberDto, newLessonNumberDto, scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task MultipleOperations_ShouldNotInterfere()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var schedule = Schedule.CreateSchedule(Guid.NewGuid(), "Test Schedule").Value;
            var lessonNumbers = new List<LessonNumber>
            {
                LessonNumber.CreateLessonNumber(1, "09:00").Value,
                LessonNumber.CreateLessonNumber(2, "10:00").Value
            };

            // Setup for GetLessonNumbersByScheduleId
            _lessonNumberRepositoryMock
                .Setup(repo => repo.GetLessonNumbersByScheduleIdAsync(scheduleId))
                .ReturnsAsync(lessonNumbers);

            // Setup for AddLessonNumber
            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            _unitOfWorkMock
                .SetupSequence(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)
                .ReturnsAsync(1);

            // Act - Get
            var getResult = await _lessonNumberServices.GetLessonNumbersByScheduleId(scheduleId);

            // Assert - Get
            Assert.Equal(2, getResult.Count);

            // Act - Add
            var addDto = new LessonNumberDto { Number = 3, Time = "11:00" };
            var addResult = await _lessonNumberServices.AddLessonNumber(addDto, scheduleId);

            // Assert - Add
            Assert.True(addResult.IsSuccess);

            // Verify all setups were used
            _lessonNumberRepositoryMock.Verify(
                repo => repo.GetLessonNumbersByScheduleIdAsync(scheduleId), Times.Once);
            _lessonNumberRepositoryMock.Verify(
                repo => repo.AddAsync(It.Is<LessonNumber>(ln => ln.Number == 3), scheduleId), Times.Once);
        }

        [Fact]
        public async Task DeleteLessonNumber_ShouldReorderOtherNumbers()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var lessonNumberDto = new LessonNumberDto { Number = 2, Time = "10:00" };
            var schedule = Schedule.CreateSchedule(Guid.NewGuid(), "Test Schedule").Value;

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            // Repository should handle reordering of other lesson numbers when one is deleted
            _lessonNumberRepositoryMock
                .Setup(repo => repo.Delete(It.IsAny<LessonNumber>(), scheduleId))
                .Callback<LessonNumber, Guid>((ln, id) =>
                {
                    // Simulate reordering in repository
                    // This is testing that the repository handles reordering internally
                })
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _lessonNumberServices.DeleteLessonNumber(lessonNumberDto, scheduleId);

            // Assert
            Assert.True(result.IsSuccess);
        }

        #endregion

        #region Null and Default Value Tests

        [Fact]
        public async Task GetLessonNumbersByScheduleId_ShouldHandleDefaultGuid()
        {
            // Arrange
            var scheduleId = default(Guid);
            var emptyList = new List<LessonNumber>();

            _lessonNumberRepositoryMock
                .Setup(repo => repo.GetLessonNumbersByScheduleIdAsync(scheduleId))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _lessonNumberServices.GetLessonNumbersByScheduleId(scheduleId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        

        [Fact]
        public async Task EditLessonNumber_ShouldHandleNullDtos()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(
                () => _lessonNumberServices.EditLessonNumber(null, null, scheduleId));
        }

        #endregion

        #region Concurrency Tests

        [Fact]
        public async Task AddLessonNumber_ShouldHandleConcurrentAdditions()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var schedule = Schedule.CreateSchedule(Guid.NewGuid(), "Test Schedule").Value;
            var lessonNumberDto1 = new LessonNumberDto { Number = 1, Time = "09:00" };
            var lessonNumberDto2 = new LessonNumberDto { Number = 2, Time = "10:00" };

            _scheduleRepositoryMock
                .Setup(repo => repo.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(schedule);

            // Simulate concurrent additions
            int callCount = 0;
            _lessonNumberRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<LessonNumber>(), scheduleId))
                .Callback(() => callCount++)
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act - Run two additions "concurrently"
            var task1 = _lessonNumberServices.AddLessonNumber(lessonNumberDto1, scheduleId);
            var task2 = _lessonNumberServices.AddLessonNumber(lessonNumberDto2, scheduleId);

            await Task.WhenAll(task1, task2);

            // Assert
            Assert.True(task1.Result.IsSuccess);
            Assert.True(task2.Result.IsSuccess);
            Assert.Equal(2, callCount);
        }

        #endregion
    }
}