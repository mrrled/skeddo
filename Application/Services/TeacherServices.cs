using Application.DtoModels;
using Application.DtoExtensions;
using Application.IServices;
using Domain.Models;
using Domain.IRepositories;

namespace Application.Services;

public class TeacherServices(
    ITeacherRepository teacherRepository,
    ISchoolSubjectRepository schoolSubjectRepository,
    IStudyGroupRepository studyGroupRepository,
    IUnitOfWork unitOfWork) : ITeacherServices
{
    public async Task<List<TeacherDto>> FetchTeachersFromBackendAsync()
    {
        var teacherList = await teacherRepository.GetTeacherListAsync(1);
        return teacherList.ToTeachersDto();
    }

    public async Task<TeacherDto> GetTeacherById(int id)
    {
        var teacher = await teacherRepository.GetTeacherByIdAsync(id);
        return teacher.ToTeacherDto();
    }

    public async Task AddTeacher(TeacherDto teacherDto)
    {
        var schoolSubjects =
            await schoolSubjectRepository.GetSchoolSubjectListByIdsAsync(teacherDto.SchoolSubjects.Select(x => x.Id)
                .Distinct().ToList());
        var studyGroups =
            await studyGroupRepository.GetStudyGroupListByIdsAsync(teacherDto.StudyGroups.Select(x => x.Id).Distinct()
                .ToList());
        var teacher = Teacher.CreateTeacher(teacherDto.Id, teacherDto.Name, teacherDto.Surname,
            teacherDto.Patronymic, schoolSubjects, studyGroups);
        await teacherRepository.AddAsync(teacher, 1);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditTeacher(TeacherDto teacherDto)
    {
        var schoolSubjects =
            await schoolSubjectRepository.GetSchoolSubjectListByIdsAsync(teacherDto.SchoolSubjects.Select(x => x.Id)
                .Distinct().ToList());
        var studyGroups =
            await studyGroupRepository.GetStudyGroupListByIdsAsync(teacherDto.StudyGroups.Select(x => x.Id).Distinct()
                .ToList());
        var teacher = await teacherRepository.GetTeacherByIdAsync(teacherDto.Id);
        if (teacher is null)
            throw new ArgumentException($"Teacher with id {teacherDto.Id} not found");
        teacher.SetName(teacherDto.Name);
        teacher.SetSurname(teacherDto.Surname);
        teacher.SetPatronymic(teacherDto.Patronymic);
        teacher.SetDescription(teacherDto.Description);
        teacher.SetSchoolSubjects(schoolSubjects);
        teacher.SetStudyGroups(studyGroups);
        await teacherRepository.UpdateAsync(teacher);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteTeacher(TeacherDto teacherDto)
    {
        var teacher = await teacherRepository.GetTeacherByIdAsync(teacherDto.Id);
        if (teacher is null)
            throw new ArgumentException($"Teacher with id {teacherDto.Id} not found");
        await teacherRepository.Delete(teacher);
        await unitOfWork.SaveChangesAsync();
    }
}