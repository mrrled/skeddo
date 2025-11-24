using Domain.Models;

namespace Domain.Repositories;

public interface ITeacherRepository
{
    Task<List<Teacher>> GetTeacherListAsync();
    Task<Teacher> GetTeacherByIdAsync(int id);
    Task AddAsync(Teacher teacher);
    Task UpdateAsync(Teacher teacher);
    Task Delete(Teacher teacher);
}