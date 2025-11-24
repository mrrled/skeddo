using Application.DtoModels;
using Application.Extensions;
using Domain;
using Domain.Models;

namespace Application.Services;

public class TeacherServices(IScheduleRepository repository, IUnitOfWork unitOfWork) : ITeacherServices
{
    public async Task<List<TeacherDto>> FetchTeachersFromBackendAsync()
    {
        var teacherList = await repository.GetTeacherListAsync();
        return teacherList.ToTeacherDto();
    }

    public async Task<TeacherDto> GetTeacherById(int id)
    {
        var teacher = await repository.GetTeacherByIdAsync(id);
        return teacher.ToTeacherDto();
    }

    public async Task AddTeacher(TeacherDto teacherDto)
    {
        var teacher = Schedule.CreateTeacher(teacherDto.Id, teacherDto.Name, teacherDto.Surname,
            teacherDto.Patronymic, teacherDto.SchoolSubjects, teacherDto.StudyGroups);
        await repository.AddAsync(teacher);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditTeacher(TeacherDto teacherDto)
    {
        var teacher = await repository.GetTeacherByIdAsync(teacherDto.Id);
        teacher.Update(teacherDto.Name, teacherDto.Surname, teacherDto.Patronymic,
            teacherDto.SchoolSubjects,
            teacherDto.StudyGroups);
        await repository.UpdateAsync(teacher);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteTeacher(TeacherDto teacherDto)
    {
        var teacher = await repository.GetTeacherByIdAsync(teacherDto.Id);
        await repository.Delete(teacher);
        await unitOfWork.SaveChangesAsync();
    }
}