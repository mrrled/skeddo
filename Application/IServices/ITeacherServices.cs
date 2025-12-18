using Application.DtoModels;
using Domain;
using Domain.Models;

namespace Application.IServices;

public interface ITeacherServices
{
    public Task<List<TeacherDto>> FetchTeachersFromBackendAsync();
    public Task<Result<TeacherDto>> GetTeacherById(Guid id);
    public Task<Result<TeacherDto>> AddTeacher(CreateTeacherDto teacherDto);
    public Task<Result> EditTeacher(TeacherDto teacherDto);
    public Task<Result> DeleteTeacher(TeacherDto teacherDto);
}