using Domain.Models;
using Infrastructure.DboExtensions;
using Infrastructure.DboModels;
using Xunit;

namespace Tests.RepositoryTests
{
    public class ClassroomExtensionsTests
    {
        [Fact]
        public void ToClassroom_ValidDbo_ReturnsClassroom()
        {
            // Arrange
            var dbo = new ClassroomDbo
            {
                Id = Guid.NewGuid(),
                Name = "101",
                ScheduleGroupId = 1
            };

            // Act
            var result = dbo.ToClassroom();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dbo.Id, result.Id);
            Assert.Equal(dbo.Name, result.Name);
        }

        [Fact]
        public void ToClassroomDbo_ValidClassroom_ReturnsDbo()
        {
            // Arrange
            var classroom = Classroom.CreateClassroom(Guid.NewGuid(), "202", "Описание").Value;

            // Act
            var result = classroom.ToClassroomDbo();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(classroom.Id, result.Id);
            Assert.Equal(classroom.Name, result.Name);
        }

        [Fact]
        public void ToClassrooms_ValidDboList_ReturnsClassroomList()
        {
            // Arrange
            var dbos = new List<ClassroomDbo>
            {
                new() { Id = Guid.NewGuid(), Name = "101", ScheduleGroupId = 1 },
                new() { Id = Guid.NewGuid(), Name = "102", ScheduleGroupId = 1 }
            };

            // Act
            var result = dbos.ToClassrooms();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, Assert.NotNull);
        }

        [Fact]
        public void ToClassrooms_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            var emptyList = new List<ClassroomDbo>();

            // Act
            var result = emptyList.ToClassrooms();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void ToClassrooms_NullList_ReturnsEmptyList()
        {
            // Arrange
            List<ClassroomDbo> nullList = null;

            // Act
            var result = nullList.ToClassrooms();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}