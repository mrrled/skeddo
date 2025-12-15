using Application;
using Application.DtoModels;
using Application.Services;
using Domain.IRepositories;
using Domain.Models;
using Moq;
using Xunit;
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

namespace Tests.ServicesTests
{
    public class TeacherServicesTests
    {
        private readonly Mock<ITeacherRepository> _teacherRepositoryMock;
        private readonly Mock<ISchoolSubjectRepository> _schoolSubjectRepositoryMock;
        private readonly Mock<IStudyGroupRepository> _studyGroupRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly TeacherServices _teacherServices;

        public TeacherServicesTests()
        {
            _teacherRepositoryMock = new Mock<ITeacherRepository>();
            _schoolSubjectRepositoryMock = new Mock<ISchoolSubjectRepository>();
            _studyGroupRepositoryMock = new Mock<IStudyGroupRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            
            _teacherServices = new TeacherServices(
                _teacherRepositoryMock.Object,
                _schoolSubjectRepositoryMock.Object,
                _studyGroupRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task FetchTeachersFromBackendAsync_ShouldReturnTeacherDtos()
        {
            // Arrange
            var teachers = new List<Teacher>
            {
                Teacher.CreateTeacher(1, "John", "Doe", "Smith",
                    [], []),
                Teacher.CreateTeacher(2, "Jane", "Smith", "Doe",
                    [], [])
            };

            _teacherRepositoryMock
                .Setup(repo => repo.GetTeacherListAsync(1))
                .ReturnsAsync(teachers);

            // Act
            var result = await _teacherServices.FetchTeachersFromBackendAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("John", result[0].Name);
            Assert.Equal("Doe", result[0].Surname);
            Assert.Equal("Smith", result[0].Patronymic);
            Assert.Equal("Jane", result[1].Name);
            _teacherRepositoryMock.Verify(repo => repo.GetTeacherListAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetTeacherById_ShouldReturnTeacherDto_WhenTeacherExists()
        {
            // Arrange
            var teacher = Teacher.CreateTeacher(1, "John", "Doe", "Smith", 
                new List<SchoolSubject>(), new List<StudyGroup>());

            _teacherRepositoryMock
                .Setup(repo => repo.GetTeacherByIdAsync(1))
                .ReturnsAsync(teacher);

            // Act
            var result = await _teacherServices.GetTeacherById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("John", result.Name);
            Assert.Equal("Doe", result.Surname);
            Assert.Equal("Smith", result.Patronymic);
            _teacherRepositoryMock.Verify(repo => repo.GetTeacherByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task AddTeacher_ShouldAddTeacherSuccessfully()
        {
            // Arrange
            var teacherDto = new TeacherDto
            {
                Id = 1,
                Name = "John",
                Surname = "Doe",
                Patronymic = "Smith",
                Description = "Math teacher",
                SchoolSubjects = [new SchoolSubjectDto { Id = 1, Name = "Mathematics" }],
                StudyGroups = [new StudyGroupDto { Id = 1, Name = "Group A" }]
            };

            var schoolSubjects = new List<SchoolSubject>
            {
                SchoolSubject.CreateSchoolSubject(1, "Mathematics")
            };

            var studyGroups = new List<StudyGroup>
            {
                StudyGroup.CreateStudyGroup(1, "Group A")
            };

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectListByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(schoolSubjects);

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupListByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(studyGroups);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _teacherServices.AddTeacher(teacherDto);

            // Assert
            _teacherRepositoryMock.Verify(repo => repo.AddAsync(
                It.Is<Teacher>(t => 
                    t.Id == 1 && 
                    t.Name == "John" && 
                    t.Surname == "Doe" && 
                    t.Patronymic == "Smith"),
                1), Times.Once);

            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddTeacher_ShouldThrowException_WhenSchoolSubjectNotFound()
        {
            // Arrange
            var teacherDto = new TeacherDto
            {
                Id = 1,
                Name = "John",
                Surname = "Doe",
                Patronymic = "Smith",
                SchoolSubjects = [new SchoolSubjectDto { Id = 999 }],
                StudyGroups = []
            };

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectListByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<SchoolSubject>());

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _teacherServices.AddTeacher(teacherDto));
        }

        [Fact]
        public async Task EditTeacher_ShouldUpdateTeacherSuccessfully()
        {
            // Arrange
            var teacherDto = new TeacherDto
            {
                Id = 1,
                Name = "John",
                Surname = "Doe",
                Patronymic = "Smith",
                Description = "Updated description",
                SchoolSubjects = [new SchoolSubjectDto { Id = 1, Name = "Mathematics" }],
                StudyGroups = [new StudyGroupDto { Id = 1, Name = "Group A" }]
            };

            var existingTeacher = Teacher.CreateTeacher(1, "Old", "Name", "Patronymic",
                new List<SchoolSubject>(), new List<StudyGroup>());

            var schoolSubjects = new List<SchoolSubject>
            {
                SchoolSubject.CreateSchoolSubject(1, "Mathematics")
            };

            var studyGroups = new List<StudyGroup>
            {
                StudyGroup.CreateStudyGroup(1, "Group A")
            };

            _teacherRepositoryMock
                .Setup(repo => repo.GetTeacherByIdAsync(1))
                .ReturnsAsync(existingTeacher);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectListByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(schoolSubjects);

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupListByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(studyGroups);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _teacherServices.EditTeacher(teacherDto);

            // Assert
            _teacherRepositoryMock.Verify(repo => repo.UpdateAsync(
                It.Is<Teacher>(t => 
                    t.Id == 1 && 
                    t.Name == "John" && 
                    t.Surname == "Doe" && 
                    t.Patronymic == "Smith" &&
                    t.Description == "Updated description")), 
                Times.Once);

            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EditTeacher_ShouldThrowException_WhenTeacherNotFound()
        {
            // Arrange
            var teacherDto = new TeacherDto { Id = 999 };

            _teacherRepositoryMock
                .Setup(repo => repo.GetTeacherByIdAsync(999))
                .ReturnsAsync((Teacher)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _teacherServices.EditTeacher(teacherDto));
        }

        [Fact]
        public async Task DeleteTeacher_ShouldDeleteTeacherSuccessfully()
        {
            // Arrange
            var teacherDto = new TeacherDto { Id = 1 };
            
            var teacher = Teacher.CreateTeacher(1, "John", "Doe", "Smith",
                new List<SchoolSubject>(), new List<StudyGroup>());

            _teacherRepositoryMock
                .Setup(repo => repo.GetTeacherByIdAsync(1))
                .ReturnsAsync(teacher);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _teacherServices.DeleteTeacher(teacherDto);

            // Assert
            _teacherRepositoryMock.Verify(repo => repo.Delete(teacher), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteTeacher_ShouldThrowException_WhenTeacherNotFound()
        {
            // Arrange
            var teacherDto = new TeacherDto { Id = 999 };

            _teacherRepositoryMock
                .Setup(repo => repo.GetTeacherByIdAsync(999))
                .ReturnsAsync((Teacher)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _teacherServices.DeleteTeacher(teacherDto));
        }

        [Fact]
        public async Task AddTeacher_ShouldHandleEmptyLists()
        {
            // Arrange
            var teacherDto = new TeacherDto
            {
                Id = 1,
                Name = "John",
                Surname = "Doe",
                Patronymic = "Smith",
                SchoolSubjects = new List<SchoolSubjectDto>(),
                StudyGroups = new List<StudyGroupDto>()
            };

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectListByIdsAsync(new List<int>()))
                .ReturnsAsync(new List<SchoolSubject>());

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupListByIdsAsync(new List<int>()))
                .ReturnsAsync(new List<StudyGroup>());

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _teacherServices.AddTeacher(teacherDto);

            // Assert
            _teacherRepositoryMock.Verify(repo => repo.AddAsync(
                It.Is<Teacher>(t => 
                    t.Id == 1 && 
                    t.SchoolSubjects.Count == 0 && 
                    t.StudyGroups.Count == 0),
                1), Times.Once);
        }

        [Fact]
        public async Task EditTeacher_ShouldHandleNullDescription()
        {
            // Arrange
            var teacherDto = new TeacherDto
            {
                Id = 1,
                Name = "John",
                Surname = "Doe",
                Patronymic = "Smith",
                Description = null,
                SchoolSubjects = new List<SchoolSubjectDto>(),
                StudyGroups = new List<StudyGroupDto>()
            };

            var existingTeacher = Teacher.CreateTeacher(1, "Old", "Name", "Patronymic",
                new List<SchoolSubject>(), new List<StudyGroup>());

            _teacherRepositoryMock
                .Setup(repo => repo.GetTeacherByIdAsync(1))
                .ReturnsAsync(existingTeacher);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectListByIdsAsync(new List<int>()))
                .ReturnsAsync(new List<SchoolSubject>());

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupListByIdsAsync(new List<int>()))
                .ReturnsAsync(new List<StudyGroup>());

            // Act & Assert
            // Note: Teacher.SetDescription throws ArgumentNullException for null description
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _teacherServices.EditTeacher(teacherDto));
        }

        [Fact]
        public async Task AddTeacher_ShouldHandleDuplicateIds()
        {
            // Arrange
            var teacherDto = new TeacherDto
            {
                Id = 1,
                Name = "John",
                Surname = "Doe",
                Patronymic = "Smith",
                SchoolSubjects =
                [
                    new SchoolSubjectDto { Id = 1 },
                    new SchoolSubjectDto { Id = 1 }
                ],
                StudyGroups =
                [
                    new StudyGroupDto { Id = 2 },
                    new StudyGroupDto { Id = 2 }
                ]
            };

            var schoolSubjects = new List<SchoolSubject>
            {
                SchoolSubject.CreateSchoolSubject(1, "Math")
            };

            var studyGroups = new List<StudyGroup>
            {
                StudyGroup.CreateStudyGroup(2, "Group A")
            };

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectListByIdsAsync(new List<int> { 1 }))
                .ReturnsAsync(schoolSubjects);

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupListByIdsAsync(new List<int> { 2 }))
                .ReturnsAsync(studyGroups);

            // Act
            await _teacherServices.AddTeacher(teacherDto);

            // Assert
            _schoolSubjectRepositoryMock.Verify(repo => 
                repo.GetSchoolSubjectListByIdsAsync(It.Is<List<int>>(list => list.Count == 1 && list[0] == 1)), 
                Times.Once);
        }
    }
}