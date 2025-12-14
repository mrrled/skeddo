using Application.DtoModels;
using Domain.Models;

namespace Application.IServices;

public interface ITeacherServices
{
    public Task<List<TeacherDto>> FetchTeachersFromBackendAsync();
    public Task<TeacherDto> GetTeacherById(Guid id);
    public Task<TeacherDto> AddTeacher(CreateTeacherDto teacherDto);
    public Task EditTeacher(TeacherDto teacherDto);
    public Task DeleteTeacher(TeacherDto teacherDto);
}