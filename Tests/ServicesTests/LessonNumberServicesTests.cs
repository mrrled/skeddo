using Application.DtoModels;
using Application.Services;
using Domain.IRepositories;
using Domain.Models;
using Application;
using Moq;
using Xunit;
// ReSharper disable PreferConcreteValueOverDefault

namespace Tests.ServicesTests
{
    public class LessonNumberServicesTests
    {
        private readonly Mock<ILessonNumberRepository> _mockLessonNumberRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly LessonNumberServices _lessonNumberServices;

        public LessonNumberServicesTests()
        {
            _mockLessonNumberRepository = new Mock<ILessonNumberRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _lessonNumberServices = new LessonNumberServices(
                _mockLessonNumberRepository.Object,
                _mockUnitOfWork.Object
            );
        }

        #region GetLessonNumbersByScheduleId Tests

        [Fact]
        public async Task GetLessonNumbersByScheduleId_ShouldReturnLessonNumbers_WhenScheduleExists()
        {
            // Arrange
            var scheduleId = 1;
            var lessonNumbers = new List<LessonNumber>
            {
                LessonNumber.CreateLessonNumber(1, "08:00-08:45"),
                LessonNumber.CreateLessonNumber(2, "09:00-09:45"),
                LessonNumber.CreateLessonNumber(3, "10:00-10:45")
            };

            _mockLessonNumberRepository
                .Setup(repo => repo.GetLessonNumbersByScheduleIdAsync(scheduleId))
                .ReturnsAsync(lessonNumbers);

            // Act
            var result = await _lessonNumberServices.GetLessonNumbersByScheduleId(scheduleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal(1, result[0].Number);
            Assert.Equal("08:00-08:45", result[0].Time);
            Assert.Equal(2, result[1].Number);
            Assert.Equal(3, result[2].Number);

            _mockLessonNumberRepository.Verify(
                repo => repo.GetLessonNumbersByScheduleIdAsync(scheduleId),
                Times.Once
            );
        }

        [Fact]
        public async Task GetLessonNumbersByScheduleId_ShouldReturnEmptyList_WhenNoLessonNumbersExist()
        {
            // Arrange
            var scheduleId = 1;
            var emptyList = new List<LessonNumber>();

            _mockLessonNumberRepository
                .Setup(repo => repo.GetLessonNumbersByScheduleIdAsync(scheduleId))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _lessonNumberServices.GetLessonNumbersByScheduleId(scheduleId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetLessonNumbersByScheduleId_ShouldThrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            var scheduleId = 1;
            
            _mockLessonNumberRepository
                .Setup(repo => repo.GetLessonNumbersByScheduleIdAsync(scheduleId))
                .ThrowsAsync(new NullReferenceException("Schedule not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() =>
                _lessonNumberServices.GetLessonNumbersByScheduleId(scheduleId)
            );
        }

        #endregion

        #region AddLessonNumber Tests

        [Fact]
        public async Task AddLessonNumber_ShouldAddLessonNumberAndSaveChanges_WhenValidInput()
        {
            // Arrange
            var scheduleId = 1;
            var lessonNumberDto = new LessonNumberDto
            {
                Number = 4,
                Time = "11:00-11:45"
            };

            _mockLessonNumberRepository
                .Setup(repo => repo.AddAsync(It.IsAny<LessonNumber>(), scheduleId))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(uow => uow.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            await _lessonNumberServices.AddLessonNumber(lessonNumberDto, scheduleId);

            // Assert
            _mockLessonNumberRepository.Verify(
                repo => repo.AddAsync(
                    It.Is<LessonNumber>(ln => 
                        ln.Number == lessonNumberDto.Number && 
                        ln.Time == lessonNumberDto.Time
                    ),
                    scheduleId
                ),
                Times.Once
            );

            _mockUnitOfWork.Verify(
                uow => uow.SaveChangesAsync(default),
                Times.Once
            );
        }

        [Theory]
        [InlineData(0, "08:00-08:45")]
        [InlineData(-1, "08:00-08:45")]
        [InlineData(1, null)]
        [InlineData(1, "")]
        public async Task AddLessonNumber_ShouldCreateLessonNumberWithVariousInputs(int number, string time)
        {
            // Arrange
            var scheduleId = 1;
            var lessonNumberDto = new LessonNumberDto
            {
                Number = number,
                Time = time
            };

            _mockLessonNumberRepository
                .Setup(repo => repo.AddAsync(It.IsAny<LessonNumber>(), scheduleId))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(uow => uow.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            await _lessonNumberServices.AddLessonNumber(lessonNumberDto, scheduleId);

            // Assert
            _mockLessonNumberRepository.Verify(
                repo => repo.AddAsync(
                    It.Is<LessonNumber>(ln => 
                        ln.Number == number && 
                        ln.Time == time
                    ),
                    scheduleId
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task AddLessonNumber_ShouldThrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            var scheduleId = 1;
            var lessonNumberDto = new LessonNumberDto
            {
                Number = 1,
                Time = "08:00-08:45"
            };

            _mockLessonNumberRepository
                .Setup(repo => repo.AddAsync(It.IsAny<LessonNumber>(), scheduleId))
                .ThrowsAsync(new NullReferenceException("Schedule not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() =>
                _lessonNumberServices.AddLessonNumber(lessonNumberDto, scheduleId)
            );

            _mockUnitOfWork.Verify(
                uow => uow.SaveChangesAsync(default),
                Times.Never
            );
        }

        [Fact]
        public async Task AddLessonNumber_ShouldNotSaveChanges_WhenRepositoryThrowsException()
        {
            // Arrange
            var scheduleId = 1;
            var lessonNumberDto = new LessonNumberDto
            {
                Number = 1,
                Time = "08:00-08:45"
            };

            _mockLessonNumberRepository
                .Setup(repo => repo.AddAsync(It.IsAny<LessonNumber>(), scheduleId))
                .ThrowsAsync(new InvalidOperationException());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _lessonNumberServices.AddLessonNumber(lessonNumberDto, scheduleId)
            );

            _mockUnitOfWork.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        #endregion

        #region EditLessonNumber Tests

        [Fact]
        public async Task EditLessonNumber_ShouldUpdateLessonNumberAndSaveChanges_WhenValidInput()
        {
            // Arrange
            var scheduleId = 1;
            var oldLessonNumberDto = new LessonNumberDto
            {
                Number = 1,
                Time = "08:00-08:45"
            };
            var newLessonNumberDto = new LessonNumberDto
            {
                Number = 1,
                Time = "08:00-09:00" // Updated time
            };

            _mockLessonNumberRepository
                .Setup(repo => repo.UpdateAsync(
                    It.IsAny<LessonNumber>(), 
                    It.IsAny<LessonNumber>(), 
                    scheduleId
                ))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(uow => uow.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            await _lessonNumberServices.EditLessonNumber(
                oldLessonNumberDto, 
                newLessonNumberDto, 
                scheduleId
            );

            // Assert
            _mockLessonNumberRepository.Verify(
                repo => repo.UpdateAsync(
                    It.Is<LessonNumber>(oldLn => 
                        oldLn.Number == oldLessonNumberDto.Number && 
                        oldLn.Time == oldLessonNumberDto.Time
                    ),
                    It.Is<LessonNumber>(newLn => 
                        newLn.Number == newLessonNumberDto.Number && 
                        newLn.Time == newLessonNumberDto.Time
                    ),
                    scheduleId
                ),
                Times.Once
            );

            _mockUnitOfWork.Verify(
                uow => uow.SaveChangesAsync(default),
                Times.Once
            );
        }

        [Fact]
        public async Task EditLessonNumber_ShouldHandleDifferentNumbers_WhenLessonNumberChanged()
        {
            // Arrange
            var scheduleId = 1;
            var oldLessonNumberDto = new LessonNumberDto
            {
                Number = 1,
                Time = "08:00-08:45"
            };
            var newLessonNumberDto = new LessonNumberDto
            {
                Number = 2, // Different number
                Time = "09:00-09:45"
            };

            _mockLessonNumberRepository
                .Setup(repo => repo.UpdateAsync(
                    It.IsAny<LessonNumber>(), 
                    It.IsAny<LessonNumber>(), 
                    scheduleId
                ))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(uow => uow.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            await _lessonNumberServices.EditLessonNumber(
                oldLessonNumberDto, 
                newLessonNumberDto, 
                scheduleId
            );

            // Assert
            _mockLessonNumberRepository.Verify(
                repo => repo.UpdateAsync(
                    It.Is<LessonNumber>(oldLn => oldLn.Number == 1),
                    It.Is<LessonNumber>(newLn => newLn.Number == 2),
                    scheduleId
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task EditLessonNumber_ShouldThrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            var scheduleId = 1;
            var oldLessonNumberDto = new LessonNumberDto
            {
                Number = 1,
                Time = "08:00-08:45"
            };
            var newLessonNumberDto = new LessonNumberDto
            {
                Number = 1,
                Time = "08:00-09:00"
            };

            _mockLessonNumberRepository
                .Setup(repo => repo.UpdateAsync(
                    It.IsAny<LessonNumber>(), 
                    It.IsAny<LessonNumber>(), 
                    scheduleId
                ))
                .ThrowsAsync(new NullReferenceException("Schedule or lesson number not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() =>
                _lessonNumberServices.EditLessonNumber(
                    oldLessonNumberDto, 
                    newLessonNumberDto, 
                    scheduleId
                )
            );

            _mockUnitOfWork.Verify(
                uow => uow.SaveChangesAsync(default),
                Times.Never
            );
        }

        #endregion

        #region DeleteLessonNumber Tests

        [Fact]
        public async Task DeleteLessonNumber_ShouldDeleteLessonNumberAndSaveChanges_WhenValidInput()
        {
            // Arrange
            var scheduleId = 1;
            var lessonNumberDto = new LessonNumberDto
            {
                Number = 1,
                Time = "08:00-08:45"
            };

            _mockLessonNumberRepository
                .Setup(repo => repo.Delete(
                    It.IsAny<LessonNumber>(), 
                    scheduleId
                ))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(uow => uow.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            await _lessonNumberServices.DeleteLessonNumber(lessonNumberDto, scheduleId);

            // Assert
            _mockLessonNumberRepository.Verify(
                repo => repo.Delete(
                    It.Is<LessonNumber>(ln => 
                        ln.Number == lessonNumberDto.Number && 
                        ln.Time == lessonNumberDto.Time
                    ),
                    scheduleId
                ),
                Times.Once
            );

            _mockUnitOfWork.Verify(
                uow => uow.SaveChangesAsync(default),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteLessonNumber_ShouldHandleNullTime_WhenLessonNumberHasNoTime()
        {
            // Arrange
            var scheduleId = 1;
            var lessonNumberDto = new LessonNumberDto
            {
                Number = 1,
                Time = null
            };

            _mockLessonNumberRepository
                .Setup(repo => repo.Delete(
                    It.IsAny<LessonNumber>(), 
                    scheduleId
                ))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(uow => uow.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            await _lessonNumberServices.DeleteLessonNumber(lessonNumberDto, scheduleId);

            // Assert
            _mockLessonNumberRepository.Verify(
                repo => repo.Delete(
                    It.Is<LessonNumber>(ln => 
                        ln.Number == 1 && 
                        ln.Time == null
                    ),
                    scheduleId
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteLessonNumber_ShouldThrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            var scheduleId = 1;
            var lessonNumberDto = new LessonNumberDto
            {
                Number = 1,
                Time = "08:00-08:45"
            };

            _mockLessonNumberRepository
                .Setup(repo => repo.Delete(
                    It.IsAny<LessonNumber>(), 
                    scheduleId
                ))
                .ThrowsAsync(new NullReferenceException("Schedule or lesson number not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() =>
                _lessonNumberServices.DeleteLessonNumber(lessonNumberDto, scheduleId)
            );

            _mockUnitOfWork.Verify(
                uow => uow.SaveChangesAsync(default),
                Times.Never
            );
        }

        [Fact]
        public async Task DeleteLessonNumber_ShouldVerifySaveChangesCalled_WhenOperationSuccessful()
        {
            // Arrange
            var scheduleId = 1;
            var lessonNumberDto = new LessonNumberDto
            {
                Number = 1,
                Time = "08:00-08:45"
            };
            int saveChangesCallCount = 0;

            _mockLessonNumberRepository
                .Setup(repo => repo.Delete(
                    It.IsAny<LessonNumber>(), 
                    scheduleId
                ))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(uow => uow.SaveChangesAsync(default))
                .Callback(() => saveChangesCallCount++)
                .ReturnsAsync(1);

            // Act
            await _lessonNumberServices.DeleteLessonNumber(lessonNumberDto, scheduleId);

            // Assert
            Assert.Equal(1, saveChangesCallCount);
        }

        #endregion

        #region Integration-style Tests

        [Fact]
        public async Task AllMethods_ShouldCallSaveChangesExactlyOnce_WhenOperationsSuccess()
        {
            // Arrange
            var scheduleId = 1;
            var lessonNumberDto = new LessonNumberDto { Number = 1, Time = "08:00-08:45" };
            var newLessonNumberDto = new LessonNumberDto { Number = 1, Time = "08:00-09:00" };
            
            var saveChangesCalls = new List<string>();

            _mockLessonNumberRepository
                .Setup(repo => repo.GetLessonNumbersByScheduleIdAsync(scheduleId))
                .ReturnsAsync(new List<LessonNumber>());

            _mockLessonNumberRepository
                .Setup(repo => repo.AddAsync(It.IsAny<LessonNumber>(), scheduleId))
                .Returns(Task.CompletedTask);

            _mockLessonNumberRepository
                .Setup(repo => repo.UpdateAsync(
                    It.IsAny<LessonNumber>(), 
                    It.IsAny<LessonNumber>(), 
                    scheduleId
                ))
                .Returns(Task.CompletedTask);

            _mockLessonNumberRepository
                .Setup(repo => repo.Delete(
                    It.IsAny<LessonNumber>(), 
                    scheduleId
                ))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(uow => uow.SaveChangesAsync(default))
                .Callback(() => saveChangesCalls.Add("SaveChanges"))
                .ReturnsAsync(1);

            // Act
            await _lessonNumberServices.AddLessonNumber(lessonNumberDto, scheduleId);
            await _lessonNumberServices.EditLessonNumber(lessonNumberDto, newLessonNumberDto, scheduleId);
            await _lessonNumberServices.DeleteLessonNumber(newLessonNumberDto, scheduleId);

            // Assert
            Assert.Equal(3, saveChangesCalls.Count);
            Assert.All(saveChangesCalls, call => Assert.Equal("SaveChanges", call));
        }

        #endregion

        #region Edge Cases and Validation Tests

        [Fact]
        public async Task AddLessonNumber_ShouldHandleMaxIntegerNumber()
        {
            // Arrange
            var scheduleId = 1;
            var lessonNumberDto = new LessonNumberDto
            {
                Number = int.MaxValue,
                Time = "23:59-00:00"
            };

            _mockLessonNumberRepository
                .Setup(repo => repo.AddAsync(It.IsAny<LessonNumber>(), scheduleId))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(uow => uow.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            await _lessonNumberServices.AddLessonNumber(lessonNumberDto, scheduleId);

            // Assert
            _mockLessonNumberRepository.Verify(
                repo => repo.AddAsync(
                    It.Is<LessonNumber>(ln => ln.Number == int.MaxValue),
                    scheduleId
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task EditLessonNumber_ShouldHandleSameDtoForOldAndNew()
        {
            // Arrange
            var scheduleId = 1;
            var sameDto = new LessonNumberDto
            {
                Number = 1,
                Time = "08:00-08:45"
            };

            _mockLessonNumberRepository
                .Setup(repo => repo.UpdateAsync(
                    It.IsAny<LessonNumber>(), 
                    It.IsAny<LessonNumber>(), 
                    scheduleId
                ))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(uow => uow.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            await _lessonNumberServices.EditLessonNumber(sameDto, sameDto, scheduleId);

            // Assert
            _mockLessonNumberRepository.Verify(
                repo => repo.UpdateAsync(
                    It.IsAny<LessonNumber>(), 
                    It.IsAny<LessonNumber>(), 
                    scheduleId
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteLessonNumber_ShouldHandleZeroNumber()
        {
            // Arrange
            var scheduleId = 1;
            var lessonNumberDto = new LessonNumberDto
            {
                Number = 0,
                Time = "00:00-00:00"
            };

            _mockLessonNumberRepository
                .Setup(repo => repo.Delete(
                    It.IsAny<LessonNumber>(), 
                    scheduleId
                ))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(uow => uow.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            await _lessonNumberServices.DeleteLessonNumber(lessonNumberDto, scheduleId);

            // Assert
            _mockLessonNumberRepository.Verify(
                repo => repo.Delete(
                    It.Is<LessonNumber>(ln => ln.Number == 0),
                    scheduleId
                ),
                Times.Once
            );
        }

        #endregion

        #region CancellationToken Tests

        [Fact]
        public async Task AllMethods_ShouldPassDefaultCancellationToken_ToSaveChanges()
        {
            // Arrange
            var scheduleId = 1;
            var lessonNumberDto = new LessonNumberDto { Number = 1, Time = "08:00-08:45" };
            var newLessonNumberDto = new LessonNumberDto { Number = 1, Time = "08:00-09:00" };

            CancellationToken capturedToken = default;

            _mockLessonNumberRepository
                .Setup(repo => repo.AddAsync(It.IsAny<LessonNumber>(), scheduleId))
                .Returns(Task.CompletedTask);

            _mockLessonNumberRepository
                .Setup(repo => repo.UpdateAsync(
                    It.IsAny<LessonNumber>(), 
                    It.IsAny<LessonNumber>(), 
                    scheduleId
                ))
                .Returns(Task.CompletedTask);

            _mockLessonNumberRepository
                .Setup(repo => repo.Delete(
                    It.IsAny<LessonNumber>(), 
                    scheduleId
                ))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Callback<CancellationToken>(ct => capturedToken = ct)
                .ReturnsAsync(1);

            // Act
            await _lessonNumberServices.AddLessonNumber(lessonNumberDto, scheduleId);
            
            // Assert for Add
            Assert.Equal(default, capturedToken);

            // Reset
            capturedToken = new CancellationToken(true);

            // Act for Edit
            await _lessonNumberServices.EditLessonNumber(lessonNumberDto, newLessonNumberDto, scheduleId);
            
            // Assert for Edit
            Assert.Equal(default, capturedToken);

            // Reset
            capturedToken = new CancellationToken(true);

            // Act for Delete
            await _lessonNumberServices.DeleteLessonNumber(newLessonNumberDto, scheduleId);
            
            // Assert for Delete
            Assert.Equal(default, capturedToken);
        }

        #endregion
    }
}