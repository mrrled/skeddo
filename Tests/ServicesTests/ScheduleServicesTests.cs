using Application.DtoModels;
using Domain.Models;
using Domain.IRepositories;
using Moq;
using Xunit;
using Application;
using Application.Services;
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

namespace Tests.ServicesTests
{
    public class ScheduleServicesTests
    {
        private readonly Mock<IScheduleRepository> _scheduleRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ScheduleServices _scheduleServices;

        public ScheduleServicesTests()
        {
            _scheduleRepositoryMock = new Mock<IScheduleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _scheduleServices = new ScheduleServices(_scheduleRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task FetchSchedulesFromBackendAsync_ShouldReturnScheduleDtos()
        {
            // Arrange
            var schedules = new List<Schedule>
            {
                Schedule.CreateSchedule(1, "Schedule 1"),
                Schedule.CreateSchedule(2, "Schedule 2")
            };
            _scheduleRepositoryMock.Setup(x => x.GetScheduleListAsync(1))
                .ReturnsAsync(schedules);

            // Act
            var result = await _scheduleServices.FetchSchedulesFromBackendAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _scheduleRepositoryMock.Verify(x => x.GetScheduleListAsync(1), Times.Once);
        }

        [Fact]
        public async Task FetchScheduleByIdAsync_WithValidId_ShouldReturnScheduleDto()
        {
            // Arrange
            var schedule = Schedule.CreateSchedule(1, "Test Schedule");
            _scheduleRepositoryMock.Setup(x => x.GetScheduleByIdAsync(1))
                .ReturnsAsync(schedule);

            // Act
            var result = await _scheduleServices.FetchScheduleByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Schedule", result.Name);
            _scheduleRepositoryMock.Verify(x => x.GetScheduleByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task FetchScheduleByIdAsync_WithInvalidId_ShouldThrowException()
        {
            // Arrange
            _scheduleRepositoryMock.Setup(x => x.GetScheduleByIdAsync(999))
                .ThrowsAsync(new InvalidOperationException());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _scheduleServices.FetchScheduleByIdAsync(999));
        }

        [Fact]
        public async Task AddSchedule_ValidScheduleDto_ShouldAddSchedule()
        {
            // Arrange
            var scheduleDto = new ScheduleDto { Id = 1, Name = "New Schedule" };
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _scheduleServices.AddSchedule(scheduleDto);

            // Assert
            _scheduleRepositoryMock.Verify(x => 
                x.AddAsync(It.Is<Schedule>(s => s.Id == 1 && s.Name == "New Schedule"), 1), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddSchedule_NullScheduleDto_ShouldThrowException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _scheduleServices.AddSchedule(null));
        }

        [Fact]
        public async Task EditSchedule_ValidScheduleDto_ShouldUpdateSchedule()
        {
            // Arrange
            var existingSchedule = Schedule.CreateSchedule(1, "Old Name");
            var scheduleDto = new ScheduleDto { Id = 1, Name = "Updated Name" };

            _scheduleRepositoryMock.Setup(x => x.GetScheduleByIdAsync(1))
                .ReturnsAsync(existingSchedule);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _scheduleServices.EditSchedule(scheduleDto);

            // Assert
            _scheduleRepositoryMock.Verify(x => x.GetScheduleByIdAsync(1), Times.Once);
            _scheduleRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Schedule>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EditSchedule_SameName_ShouldNotUpdate()
        {
            // Arrange
            var existingSchedule = Schedule.CreateSchedule(1, "Same Name");
            var scheduleDto = new ScheduleDto { Id = 1, Name = "Same Name" };

            _scheduleRepositoryMock.Setup(x => x.GetScheduleByIdAsync(1))
                .ReturnsAsync(existingSchedule);

            // Act
            await _scheduleServices.EditSchedule(scheduleDto);

            // Assert
            _scheduleRepositoryMock.Verify(x => x.GetScheduleByIdAsync(1), Times.Once);
            _scheduleRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Schedule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task EditSchedule_ScheduleNotFound_ShouldThrowArgumentException()
        {
            // Arrange
            var scheduleDto = new ScheduleDto { Id = 999, Name = "Not Found" };
            _scheduleRepositoryMock.Setup(x => x.GetScheduleByIdAsync(999))
                .ReturnsAsync((Schedule)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _scheduleServices.EditSchedule(scheduleDto));
            Assert.Contains("Schedule with id 999 not found", exception.Message);
        }

        [Fact]
        public async Task DeleteSchedule_ValidScheduleDto_ShouldDeleteSchedule()
        {
            // Arrange
            var existingSchedule = Schedule.CreateSchedule(1, "To Delete");
            var scheduleDto = new ScheduleDto { Id = 1, Name = "To Delete" };

            _scheduleRepositoryMock.Setup(x => x.GetScheduleByIdAsync(1))
                .ReturnsAsync(existingSchedule);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _scheduleServices.DeleteSchedule(scheduleDto);

            // Assert
            _scheduleRepositoryMock.Verify(x => x.GetScheduleByIdAsync(1), Times.Once);
            _scheduleRepositoryMock.Verify(x => x.Delete(existingSchedule), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteSchedule_ScheduleNotFound_ShouldThrowArgumentException()
        {
            // Arrange
            var scheduleDto = new ScheduleDto { Id = 999, Name = "Not Found" };
            _scheduleRepositoryMock.Setup(x => x.GetScheduleByIdAsync(999))
                .ReturnsAsync((Schedule)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _scheduleServices.DeleteSchedule(scheduleDto));
            Assert.Contains("Schedule with id 999 not found", exception.Message);
        }

        [Fact]
        public async Task GetScheduleByIdAsync_ValidId_ShouldReturnScheduleDto()
        {
            // Arrange
            var schedule = Schedule.CreateSchedule(1, "Test Schedule");
            _scheduleRepositoryMock.Setup(x => x.GetScheduleByIdAsync(1))
                .ReturnsAsync(schedule);

            // Act
            var result = await _scheduleServices.GetScheduleByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Schedule", result.Name);
            _scheduleRepositoryMock.Verify(x => x.GetScheduleByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetScheduleByIdAsync_InvalidId_ShouldThrowException()
        {
            // Arrange
            _scheduleRepositoryMock.Setup(x => x.GetScheduleByIdAsync(999))
                .ThrowsAsync(new InvalidOperationException("Schedule not found"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _scheduleServices.GetScheduleByIdAsync(999));
        }

        [Fact]
        public async Task EditSchedule_WithLessonsAndDrafts_ShouldHandleCollections()
        {
            // Arrange
            var schedule = Schedule.CreateSchedule(1, "Schedule with content");
            
            // Create test data
            var teacher = Teacher.CreateTeacher(1, "John", "Doe", "Smith", 
                new List<SchoolSubject>(), new List<StudyGroup>());
            var subject = SchoolSubject.CreateSchoolSubject(1, "Math");
            var group = StudyGroup.CreateStudyGroup(1, "Group A");
            var classroom = Classroom.CreateClassroom(1, "Room 101", null);
            var lessonNumber = LessonNumber.CreateLessonNumber(1, "09:00");
            
            var lesson = new Lesson(1, subject, lessonNumber, teacher, group, classroom, "Comment");
            var lessonDraft = LessonDraft.CreateFromLesson(lesson);
            
            // Use reflection to add to private collections
            var lessonsField = typeof(Schedule).GetField("_lessons", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var draftsField = typeof(Schedule).GetField("_lessonDrafts", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            var lessonsSet = new HashSet<Lesson> { lesson };
            var draftsSet = new HashSet<LessonDraft> { lessonDraft };
            
            lessonsField?.SetValue(schedule, lessonsSet);
            draftsField?.SetValue(schedule, draftsSet);
            
            var scheduleDto = new ScheduleDto { Id = 1, Name = "Updated Name" };
            
            _scheduleRepositoryMock.Setup(x => x.GetScheduleByIdAsync(1))
                .ReturnsAsync(schedule);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _scheduleServices.EditSchedule(scheduleDto);

            // Assert
            _scheduleRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Schedule>(s => 
                s.Lessons.Count == 1 && s.LessonDrafts.Count == 1)), Times.Once);
        }

        [Fact]
        public void Constructor_NullDependencies_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ScheduleServices(null, _unitOfWorkMock.Object));
            Assert.Throws<ArgumentNullException>(() => new ScheduleServices(_scheduleRepositoryMock.Object, null));
        }

        [Fact]
        public async Task FetchSchedulesFromBackendAsync_RepositoryThrowsException_ShouldPropagate()
        {
            // Arrange
            _scheduleRepositoryMock.Setup(x => x.GetScheduleListAsync(1))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _scheduleServices.FetchSchedulesFromBackendAsync());
        }

        [Fact]
        public async Task SaveChangesAsync_Fails_ShouldPropagateException()
        {
            // Arrange
            var scheduleDto = new ScheduleDto { Id = 1, Name = "Test" };
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Save failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _scheduleServices.AddSchedule(scheduleDto));
        }
    }
}