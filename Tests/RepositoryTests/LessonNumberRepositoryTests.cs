using Domain.Models;
using Infrastructure.DboExtensions;
using Infrastructure.DboModels;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;
using Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Xunit;

namespace Tests.RepositoryTests
{
    public class LessonNumberRepositoryTests
    {
        private  Mock<ScheduleDbContext> _mockContext;
        private  Mock<DbSet<LessonNumberDbo>> _mockLessonNumbersSet;
        private  Mock<DbSet<ScheduleDbo>> _mockSchedulesSet;
        private  LessonNumberRepository _repository;
        private  List<LessonNumberDbo> _mockLessonNumbers;
        private  List<ScheduleDbo> _mockSchedules;

        public LessonNumberRepositoryTests()
        {
            _mockContext = new Mock<ScheduleDbContext>();
            _mockLessonNumbersSet = new Mock<DbSet<LessonNumberDbo>>();
            _mockSchedulesSet = new Mock<DbSet<ScheduleDbo>>();

            SetupMockData();
            SetupMocks();
            _repository = new LessonNumberRepository(_mockContext.Object);
        }

        private void SetupMockData()
        {
            var scheduleId1 = Guid.NewGuid();
            var scheduleId2 = Guid.NewGuid();

            // Setup Schedules
            _mockSchedules = new List<ScheduleDbo>
            {
                new ScheduleDbo { 
                    Id = scheduleId1, 
                    Name = "Schedule 1",
                    LessonNumbers = new List<LessonNumberDbo>()
                },
                new ScheduleDbo { 
                    Id = scheduleId2, 
                    Name = "Schedule 2",
                    LessonNumbers = new List<LessonNumberDbo>()
                },
                new ScheduleDbo { 
                    Id = Guid.NewGuid(), 
                    Name = "Schedule without numbers",
                    LessonNumbers = new List<LessonNumberDbo>()
                }
            };

            // Setup Lesson Numbers for Schedule 1
            var lessonNumbersSchedule1 = new List<LessonNumberDbo>
            {
                new LessonNumberDbo { Id = Guid.NewGuid(), ScheduleId = scheduleId1, Number = 1, Time = "09:00" },
                new LessonNumberDbo { Id = Guid.NewGuid(), ScheduleId = scheduleId1, Number = 2, Time = "10:00" },
                new LessonNumberDbo { Id = Guid.NewGuid(), ScheduleId = scheduleId1, Number = 3, Time = "11:00" },
                new LessonNumberDbo { Id = Guid.NewGuid(), ScheduleId = scheduleId1, Number = 4, Time = "12:00" }
            };

            // Setup Lesson Numbers for Schedule 2
            var lessonNumbersSchedule2 = new List<LessonNumberDbo>
            {
                new LessonNumberDbo { Id = Guid.NewGuid(), ScheduleId = scheduleId2, Number = 1, Time = "08:30" },
                new LessonNumberDbo { Id = Guid.NewGuid(), ScheduleId = scheduleId2, Number = 2, Time = "09:30" }
            };

            _mockLessonNumbers = new List<LessonNumberDbo>();
            _mockLessonNumbers.AddRange(lessonNumbersSchedule1);
            _mockLessonNumbers.AddRange(lessonNumbersSchedule2);

            // Update schedules with lesson numbers
            _mockSchedules[0].LessonNumbers = lessonNumbersSchedule1;
            _mockSchedules[1].LessonNumbers = lessonNumbersSchedule2;
        }

        private void SetupMocks()
        {
            // Setup LessonNumbers DbSet
            var lessonNumbersQueryable = _mockLessonNumbers.AsQueryable();
            SetupDbSet(_mockLessonNumbersSet, lessonNumbersQueryable);

            // Setup Schedules DbSet with Include support
            var schedulesQueryable = _mockSchedules.AsQueryable();
            
            // Create a mock IIncludableQueryable
            var mockIncludableQueryable = new Mock<IIncludableQueryable<ScheduleDbo, IEnumerable<LessonNumberDbo>>>();
            
            // Setup the mock to return our queryable data
            mockIncludableQueryable.As<IQueryable<ScheduleDbo>>().Setup(m => m.Provider).Returns(schedulesQueryable.Provider);
            mockIncludableQueryable.As<IQueryable<ScheduleDbo>>().Setup(m => m.Expression).Returns(schedulesQueryable.Expression);
            mockIncludableQueryable.As<IQueryable<ScheduleDbo>>().Setup(m => m.ElementType).Returns(schedulesQueryable.ElementType);
            mockIncludableQueryable.As<IQueryable<ScheduleDbo>>().Setup(m => m.GetEnumerator()).Returns(schedulesQueryable.GetEnumerator());
            
            // Setup async support
            mockIncludableQueryable.As<IAsyncEnumerable<ScheduleDbo>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<ScheduleDbo>(schedulesQueryable.GetEnumerator()));

            // Setup DbSet to return our IIncludableQueryable when Include is called
            _mockSchedulesSet.As<IQueryable<ScheduleDbo>>().Setup(m => m.Provider).Returns(schedulesQueryable.Provider);
            _mockSchedulesSet.As<IQueryable<ScheduleDbo>>().Setup(m => m.Expression).Returns(schedulesQueryable.Expression);
            _mockSchedulesSet.As<IQueryable<ScheduleDbo>>().Setup(m => m.ElementType).Returns(schedulesQueryable.ElementType);
            _mockSchedulesSet.As<IQueryable<ScheduleDbo>>().Setup(m => m.GetEnumerator()).Returns(schedulesQueryable.GetEnumerator());
            
            _mockSchedulesSet.As<IAsyncEnumerable<ScheduleDbo>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<ScheduleDbo>(schedulesQueryable.GetEnumerator()));

            // Setup Include to return our mock IIncludableQueryable
            _mockSchedulesSet.Setup(m => m.Include(It.IsAny<Expression<Func<ScheduleDbo, IEnumerable<LessonNumberDbo>>>>()))
                .Returns(mockIncludableQueryable.Object);

            // Also setup string-based Include for FirstOrDefaultAsync
            _mockSchedulesSet.Setup(m => m.Include(It.IsAny<string>()))
                .Returns(_mockSchedulesSet.Object);

            // Setup Context
            _mockContext.Setup(c => c.LessonNumbers).Returns(_mockLessonNumbersSet.Object);
            _mockContext.Setup(c => c.Schedules).Returns(_mockSchedulesSet.Object);
        }

        private void SetupDbSet<T>(Mock<DbSet<T>> mockSet, IQueryable<T> queryable) where T : class
        {
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(queryable.GetEnumerator()));
        }

        #region GetLessonNumbersByScheduleIdAsync Tests

        [Fact]
        public async Task GetLessonNumbersByScheduleIdAsync_ValidScheduleId_ReturnsLessonNumbers()
        {
            // Arrange
            var scheduleId = _mockSchedules[0].Id;

            // Act
            var result = await _repository.GetLessonNumbersByScheduleIdAsync(scheduleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Count);
            Assert.All(result, n => Assert.Equal(scheduleId, n.ToLessonNumberDbo().ScheduleId));
        }

        [Fact]
        public async Task GetLessonNumbersByScheduleIdAsync_NonExistentScheduleId_ThrowsInvalidOperationException()
        {
            // Arrange
            var nonExistentScheduleId = Guid.NewGuid();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.GetLessonNumbersByScheduleIdAsync(nonExistentScheduleId));

            Assert.Equal($"Schedule with id {nonExistentScheduleId} not found.", exception.Message);
        }

        [Fact]
        public async Task GetLessonNumbersByScheduleIdAsync_ScheduleWithNoLessonNumbers_ReturnsEmptyList()
        {
            // Arrange
            var scheduleWithoutNumbers = _mockSchedules[2].Id;

            // Act
            var result = await _repository.GetLessonNumbersByScheduleIdAsync(scheduleWithoutNumbers);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region AddAsync Tests

        // [Fact]
        // public async Task AddAsync_ValidLessonNumber_AddsSuccessfully()
        // {
        //     // Arrange
        //     var scheduleId = _mockSchedules[0].Id;
        //     var lessonNumber = LessonNumber.CreateLessonNumber(5, "13:00").Value;
        //     var initialCount = _mockLessonNumbers.Count;
        //
        //     _mockLessonNumbersSet.Setup(m => m.AddAsync(It.IsAny<LessonNumberDbo>(), It.IsAny<CancellationToken>()))
        //         .Callback<LessonNumberDbo, CancellationToken>((dbo, token) => _mockLessonNumbers.Add(dbo))
        //         .Returns(ValueTask.FromResult((object)null));
        //
        //     // Act
        //     await _repository.AddAsync(lessonNumber, scheduleId);
        //
        //     // Assert
        //     _mockLessonNumbersSet.Verify(m => m.AddAsync(It.IsAny<LessonNumberDbo>(), It.IsAny<CancellationToken>()), Times.Once);
        //     Assert.Equal(initialCount + 1, _mockLessonNumbers.Count);
        // }

        [Fact]
        public async Task AddAsync_NonExistentSchedule_ThrowsInvalidOperationException()
        {
            // Arrange
            var nonExistentScheduleId = Guid.NewGuid();
            var lessonNumber = LessonNumber.CreateLessonNumber(1, "09:00").Value;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.AddAsync(lessonNumber, nonExistentScheduleId));

            Assert.Equal($"Schedule with id {nonExistentScheduleId} not found.", exception.Message);
        }

        // [Fact]
        // public async Task AddAsync_NullLessonNumber_AddsSuccessfully()
        // {
        //     // Arrange
        //     var scheduleId = _mockSchedules[0].Id;
        //     LessonNumber nullLessonNumber = null;
        //     var initialCount = _mockLessonNumbers.Count;
        //
        //     _mockLessonNumbersSet.Setup(m => m.AddAsync(It.IsAny<LessonNumberDbo>(), It.IsAny<CancellationToken>()))
        //         .Callback<LessonNumberDbo, CancellationToken>((dbo, token) => _mockLessonNumbers.Add(dbo))
        //         .Returns(ValueTask.FromResult((object)null));
        //
        //     // Act
        //     await _repository.AddAsync(nullLessonNumber, scheduleId);
        //
        //     // Assert
        //     _mockLessonNumbersSet.Verify(m => m.AddAsync(It.IsAny<LessonNumberDbo>(), It.IsAny<CancellationToken>()), Times.Once);
        //     Assert.Equal(initialCount + 1, _mockLessonNumbers.Count);
        // }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ValidLessonNumber_UpdatesSuccessfully()
        {
            // Arrange
            var scheduleId = _mockSchedules[0].Id;
            var oldLessonNumber = LessonNumber.CreateLessonNumber(2, "10:00").Value;
            var newLessonNumber = LessonNumber.CreateLessonNumber(2, "10:15").Value;

            // Act
            var exception = await Record.ExceptionAsync(() => 
                _repository.UpdateAsync(oldLessonNumber, newLessonNumber, scheduleId));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task UpdateAsync_NonExistentOldLessonNumber_ThrowsInvalidOperationException()
        {
            // Arrange
            var scheduleId = _mockSchedules[0].Id;
            var oldLessonNumber = LessonNumber.CreateLessonNumber(999, "99:99").Value;
            var newLessonNumber = LessonNumber.CreateLessonNumber(2, "10:15").Value;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.UpdateAsync(oldLessonNumber, newLessonNumber, scheduleId));

            Assert.Equal($"Lesson number with number {oldLessonNumber.Number} and schedule id {scheduleId} not found.", exception.Message);
        }

        [Fact]
        public async Task UpdateAsync_NullOldLessonNumber_ThrowsArgumentNullException()
        {
            // Arrange
            var scheduleId = _mockSchedules[0].Id;
            LessonNumber oldLessonNumber = null;
            var newLessonNumber = LessonNumber.CreateLessonNumber(2, "10:15").Value;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _repository.UpdateAsync(oldLessonNumber, newLessonNumber, scheduleId));
        }

        [Fact]
        public async Task UpdateAsync_NullNewLessonNumber_ThrowsArgumentNullException()
        {
            // Arrange
            var scheduleId = _mockSchedules[0].Id;
            var oldLessonNumber = LessonNumber.CreateLessonNumber(2, "10:00").Value;
            LessonNumber newLessonNumber = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _repository.UpdateAsync(oldLessonNumber, newLessonNumber, scheduleId));
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_ValidLessonNumber_DeletesSuccessfully()
        {
            // Arrange
            var scheduleId = _mockSchedules[0].Id;
            var lessonNumberToDelete = LessonNumber.CreateLessonNumber(2, "10:00").Value;
            var initialCount = _mockLessonNumbers.Count(n => n.ScheduleId == scheduleId);

            _mockLessonNumbersSet.Setup(m => m.Remove(It.IsAny<LessonNumberDbo>()))
                .Callback<LessonNumberDbo>(dbo => _mockLessonNumbers.RemoveAll(n => 
                    n.ScheduleId == scheduleId && n.Number == lessonNumberToDelete.Number));

            // Act
            await _repository.Delete(lessonNumberToDelete, scheduleId);

            // Assert
            _mockLessonNumbersSet.Verify(m => m.Remove(It.IsAny<LessonNumberDbo>()), Times.Once);
        }

        [Fact]
        public async Task Delete_NonExistentLessonNumber_ThrowsInvalidOperationException()
        {
            // Arrange
            var scheduleId = _mockSchedules[0].Id;
            var nonExistentLessonNumber = LessonNumber.CreateLessonNumber(999, "99:99").Value;

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.Delete(nonExistentLessonNumber, scheduleId));
        }

        [Fact]
        public async Task Delete_NullLessonNumber_ThrowsArgumentNullException()
        {
            // Arrange
            var scheduleId = _mockSchedules[0].Id;
            LessonNumber nullLessonNumber = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _repository.Delete(nullLessonNumber, scheduleId));
        }

        #endregion

        #region Integration Tests

        // [Fact]
        // public async Task FullCRUD_Flow_WorksCorrectly()
        // {
        //     // Arrange
        //     var scheduleId = Guid.NewGuid();
        //     _mockSchedules.Add(new ScheduleDbo 
        //     { 
        //         Id = scheduleId, 
        //         Name = "Test Schedule",
        //         LessonNumbers = new List<LessonNumberDbo>() 
        //     });
        //
        //     // 1. Add lesson number
        //     var lessonNumber = LessonNumber.CreateLessonNumber(1, "09:00").Value;
        //     
        //     _mockLessonNumbersSet.Setup(m => m.AddAsync(It.IsAny<LessonNumberDbo>(), It.IsAny<CancellationToken>()))
        //         .Callback<LessonNumberDbo, CancellationToken>((dbo, token) => _mockLessonNumbers.Add(dbo))
        //         .Returns(ValueTask.FromResult((object)null));
        //
        //     await _repository.AddAsync(lessonNumber, scheduleId);
        //
        //     // 2. Get lesson numbers
        //     var result = await _repository.GetLessonNumbersByScheduleIdAsync(scheduleId);
        //     Assert.Single(result);
        //
        //     // 3. Update lesson number
        //     var updatedLessonNumber = LessonNumber.CreateLessonNumber(1, "09:15").Value;
        //     await _repository.UpdateAsync(lessonNumber, updatedLessonNumber, scheduleId);
        //
        //     // 4. Delete lesson number
        //     _mockLessonNumbersSet.Setup(m => m.Remove(It.IsAny<LessonNumberDbo>()))
        //         .Callback<LessonNumberDbo>(dbo => _mockLessonNumbers.RemoveAll(n => n.ScheduleId == scheduleId));
        //
        //     await _repository.Delete(updatedLessonNumber, scheduleId);
        //
        //     // 5. Verify deletion
        //     var finalResult = await _repository.GetLessonNumbersByScheduleIdAsync(scheduleId);
        //     Assert.Empty(finalResult);
        // }

        #endregion

        #region Helper Classes for Async Testing

        private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;

            public TestAsyncEnumerator(IEnumerator<T> inner)
            {
                _inner = inner;
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return ValueTask.FromResult(_inner.MoveNext());
            }

            public T Current => _inner.Current;

            public ValueTask DisposeAsync()
            {
                _inner.Dispose();
                return ValueTask.CompletedTask;
            }
        }

        #endregion
    }
}