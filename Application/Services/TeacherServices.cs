using Application.DtoModels;
using Application.Extensions;
using Domain.Models;
using Domain.Repositories;

namespace Application.Services;

public class TeacherServices(ITeacherRepository teacherRepository, IUnitOfWork unitOfWork) : ITeacherServices
{
    public async Task<List<TeacherDto>> FetchTeachersFromBackendAsync()
    {
        var teacherList = await teacherRepository.GetTeacherListAsync();
        return teacherList.ToTeacherDto();
    }

    public async Task<TeacherDto> GetTeacherById(int id)
    {
        var teacher = await teacherRepository.GetTeacherByIdAsync(id);
        return teacher.ToTeacherDto();
    }

    public async Task AddTeacher(TeacherDto teacherDto)
    {
        var teacher = Schedule.CreateTeacher(teacherDto.Id, teacherDto.Name, teacherDto.Surname,
            teacherDto.Patronymic, teacherDto.SchoolSubjects, teacherDto.StudyGroups);
        await teacherRepository.AddAsync(teacher);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditTeacher(TeacherDto teacherDto)
    {
        var teacher = await teacherRepository.GetTeacherByIdAsync(teacherDto.Id);
        teacher.Update(teacherDto.Name, teacherDto.Surname, teacherDto.Patronymic,
            teacherDto.SchoolSubjects,
            teacherDto.StudyGroups);
        await teacherRepository.UpdateAsync(teacher);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteTeacher(TeacherDto teacherDto)
    {
        var teacher = await teacherRepository.GetTeacherByIdAsync(teacherDto.Id);
        await teacherRepository.Delete(teacher);
        await unitOfWork.SaveChangesAsync();
    }
}