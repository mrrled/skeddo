using Application.DtoModels;
using Domain.Models;

namespace Application.IServices;

public interface ITeacherServices
{
    public Task<List<TeacherDto>> FetchTeachersFromBackendAsync();
    public Task<TeacherDto> GetTeacherById(int id);
    public Task AddTeacher(TeacherDto teacherDto);
    public Task EditTeacher(TeacherDto teacherDto);
    public Task DeleteTeacher(TeacherDto teacherDto);
}