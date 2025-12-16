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
            var teacherId1 = Guid.NewGuid();
            var teacherId2 = Guid.NewGuid();
            var teachers = new List<Teacher>
            {
                Teacher.CreateTeacher(teacherId1, "John", "Doe", "Smith",
                    [], []),
                Teacher.CreateTeacher(teacherId2, "Jane", "Smith", "Doe",
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
            var teacherId = Guid.NewGuid();
            var teacher = Teacher.CreateTeacher(teacherId, "John", "Doe", "Smith", 
                new List<SchoolSubject>(), new List<StudyGroup>());

            _teacherRepositoryMock
                .Setup(repo => repo.GetTeacherByIdAsync(teacherId))
                .ReturnsAsync(teacher);

            // Act
            var result = await _teacherServices.GetTeacherById(teacherId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(teacherId, result.Id);
            Assert.Equal("John", result.Name);
            Assert.Equal("Doe", result.Surname);
            Assert.Equal("Smith", result.Patronymic);
            _teacherRepositoryMock.Verify(repo => repo.GetTeacherByIdAsync(teacherId), Times.Once);
        }

        [Fact]
        public async Task AddTeacher_ShouldAddTeacherSuccessfully()
        {
            // Arrange
            var schoolSubjectId = Guid.NewGuid();
            var studyGroupId = Guid.NewGuid();
            var teacherDto = new CreateTeacherDto
            {
                Name = "John",
                Surname = "Doe",
                Patronymic = "Smith",
                Description = "Math teacher",
                SchoolSubjects = [new SchoolSubjectDto { Id = schoolSubjectId, Name = "Mathematics" }],
                StudyGroups = [new StudyGroupDto { Id = studyGroupId, Name = "Group A" }]
            };

            var schoolSubjects = new List<SchoolSubject>
            {
                SchoolSubject.CreateSchoolSubject(schoolSubjectId, "Mathematics")
            };

            var studyGroups = new List<StudyGroup>
            {
                StudyGroup.CreateStudyGroup(studyGroupId, "Group A")
            };

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectListByIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(schoolSubjects);

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupListByIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(studyGroups);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _teacherServices.AddTeacher(teacherDto);

            // Assert
            _teacherRepositoryMock.Verify(repo => repo.AddAsync(
                It.Is<Teacher>(t => 
                    t.Name == "John" && 
                    t.Surname == "Doe" && 
                    t.Patronymic == "Smith"),
                1), Times.Once);

            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        // [Fact]
        // public async Task AddTeacher_ShouldThrowException_WhenSchoolSubjectNotFound()   //необязательно учителю иметь предметы
        // {
        //     // Arrange
        //     var teacherDto = new TeacherDto
        //     {
        //         Id = 1,
        //         Name = "John",
        //         Surname = "Doe",
        //         Patronymic = "Smith",
        //         SchoolSubjects = [new SchoolSubjectDto { Id = 999 }],
        //         StudyGroups = []
        //     };
        //
        //     _schoolSubjectRepositoryMock
        //         .Setup(repo => repo.GetSchoolSubjectListByIdsAsync(It.IsAny<List<int>>()))
        //         .ReturnsAsync(new List<SchoolSubject>());
        //
        //     // Act & Assert
        //     await Assert.ThrowsAsync<ArgumentException>(() => 
        //         _teacherServices.AddTeacher(teacherDto));
        // }

        [Fact]
        public async Task EditTeacher_ShouldUpdateTeacherSuccessfully()
        {
            // Arrange
            var schoolSubjectId = Guid.NewGuid();
            var studyGroupId = Guid.NewGuid();
            var teacherId = Guid.NewGuid();
            var teacherDto = new TeacherDto
            {
                Id = teacherId,
                Name = "John",
                Surname = "Doe",
                Patronymic = "Smith",
                Description = "Updated description",
                SchoolSubjects = [new SchoolSubjectDto { Id = schoolSubjectId, Name = "Mathematics" }],
                StudyGroups = [new StudyGroupDto { Id = studyGroupId, Name = "Group A" }]
            };

            var existingTeacher = Teacher.CreateTeacher(teacherId, "Old", "Name", "Patronymic",
                new List<SchoolSubject>(), new List<StudyGroup>());

            var schoolSubjects = new List<SchoolSubject>
            {
                SchoolSubject.CreateSchoolSubject(schoolSubjectId, "Mathematics")
            };

            var studyGroups = new List<StudyGroup>
            {
                StudyGroup.CreateStudyGroup(studyGroupId, "Group A")
            };

            _teacherRepositoryMock
                .Setup(repo => repo.GetTeacherByIdAsync(teacherId))
                .ReturnsAsync(existingTeacher);

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectListByIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(schoolSubjects);

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupListByIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(studyGroups);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _teacherServices.EditTeacher(teacherDto);

            // Assert
            _teacherRepositoryMock.Verify(repo => repo.UpdateAsync(
                It.Is<Teacher>(t => 
                    t.Id == teacherId && 
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
            var teacherId = Guid.NewGuid();
            var teacherDto = new TeacherDto { Id = teacherId };

            _teacherRepositoryMock
                .Setup(repo => repo.GetTeacherByIdAsync(teacherId))
                .ReturnsAsync((Teacher)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _teacherServices.EditTeacher(teacherDto));
        }

        [Fact]
        public async Task DeleteTeacher_ShouldDeleteTeacherSuccessfully()
        {
            // Arrange
            var teacherId = Guid.NewGuid();
            var teacherDto = new TeacherDto { Id = teacherId };
            
            var teacher = Teacher.CreateTeacher(teacherId, "John", "Doe", "Smith",
                new List<SchoolSubject>(), new List<StudyGroup>());

            _teacherRepositoryMock
                .Setup(repo => repo.GetTeacherByIdAsync(teacherId))
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
            var teacherId = Guid.NewGuid();
            var teacherDto = new TeacherDto { Id = teacherId };

            _teacherRepositoryMock
                .Setup(repo => repo.GetTeacherByIdAsync(teacherId))
                .ReturnsAsync((Teacher)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _teacherServices.DeleteTeacher(teacherDto));
        }

        [Fact]
        public async Task AddTeacher_ShouldHandleEmptyLists()
        {
            // Arrange
            var teacherId = Guid.NewGuid();
            var teacherDto = new CreateTeacherDto()
            {
                Name = "John",
                Surname = "Doe",
                Patronymic = "Smith",
                SchoolSubjects = new List<SchoolSubjectDto>(),
                StudyGroups = new List<StudyGroupDto>()
            };

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectListByIdsAsync(new List<Guid>()))
                .ReturnsAsync(new List<SchoolSubject>());

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupListByIdsAsync(new List<Guid>()))
                .ReturnsAsync(new List<StudyGroup>());

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _teacherServices.AddTeacher(teacherDto);

            // Assert
            _teacherRepositoryMock.Verify(repo => repo.AddAsync(
                It.Is<Teacher>(t => 
                    t.SchoolSubjects.Count == 0 && 
                    t.StudyGroups.Count == 0),
                1), Times.Once);
        }

        // [Fact]
        // public async Task EditTeacher_ShouldHandleNullDescription()
        // {
        //     // Arrange
        //     var teacherId = Guid.NewGuid();
        //     var teacherDto = new TeacherDto
        //     {
        //         Id = 1,
        //         Name = "John",
        //         Surname = "Doe",
        //         Patronymic = "Smith",
        //         Description = null,
        //         SchoolSubjects = new List<SchoolSubjectDto>(),
        //         StudyGroups = new List<StudyGroupDto>()
        //     };
        //
        //     var existingTeacher = Teacher.CreateTeacher(teacherId, "Old", "Name", "Patronymic",
        //         new List<SchoolSubject>(), new List<StudyGroup>());
        //
        //     _teacherRepositoryMock
        //         .Setup(repo => repo.GetTeacherByIdAsync(teacherId))
        //         .ReturnsAsync(existingTeacher);
        //
        //     _schoolSubjectRepositoryMock
        //         .Setup(repo => repo.GetSchoolSubjectListByIdsAsync(new List<Guid>()))
        //         .ReturnsAsync(new List<SchoolSubject>());
        //
        //     _studyGroupRepositoryMock
        //         .Setup(repo => repo.GetStudyGroupListByIdsAsync(new List<Guid>()))
        //         .ReturnsAsync(new List<StudyGroup>());
        //
        //     // Act & Assert
        //     // Note: Teacher.SetDescription throws ArgumentNullException for null description, это где-то вроде поправлено
        //     await Assert.ThrowsAsync<ArgumentNullException>(() => 
        //         _teacherServices.EditTeacher(teacherDto));
        // }

        [Fact]
        public async Task AddTeacher_ShouldHandleDuplicateIds()
        {
            // Arrange
            var schoolSubjectId = Guid.NewGuid();
            var studyGroupId = Guid.NewGuid();
            var teacherDto = new CreateTeacherDto
            {
                Name = "John",
                Surname = "Doe",
                Patronymic = "Smith",
                SchoolSubjects =
                [
                    new SchoolSubjectDto { Id = schoolSubjectId },
                    new SchoolSubjectDto { Id = schoolSubjectId }
                ],
                StudyGroups =
                [
                    new StudyGroupDto { Id = studyGroupId },
                    new StudyGroupDto { Id = studyGroupId }
                ]
            };

            var schoolSubjects = new List<SchoolSubject>
            {
                SchoolSubject.CreateSchoolSubject(schoolSubjectId, "Math")
            };

            var studyGroups = new List<StudyGroup>
            {
                StudyGroup.CreateStudyGroup(studyGroupId, "Group A")
            };

            _schoolSubjectRepositoryMock
                .Setup(repo => repo.GetSchoolSubjectListByIdsAsync(new List<Guid> { schoolSubjectId }))
                .ReturnsAsync(schoolSubjects);

            _studyGroupRepositoryMock
                .Setup(repo => repo.GetStudyGroupListByIdsAsync(new List<Guid> { studyGroupId }))
                .ReturnsAsync(studyGroups);

            // Act
            await _teacherServices.AddTeacher(teacherDto);

            // Assert
            _schoolSubjectRepositoryMock.Verify(repo => 
                repo.GetSchoolSubjectListByIdsAsync(It.Is<List<Guid>>(list => list.Count == 1 && list[0] == schoolSubjectId)), 
                Times.Once);
        }
    }
}