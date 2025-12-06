using Domain.Models;

namespace Domain.IRepositories;

public interface ITeacherRepository
{
    Task<List<Teacher>> GetTeacherListAsync(int scheduleGroupId);
    Task<Teacher> GetTeacherByIdAsync(int id);
    Task AddAsync(Teacher teacher, int scheduleGroupId);
    Task UpdateAsync(Teacher teacher);
    Task Delete(Teacher teacher);
}