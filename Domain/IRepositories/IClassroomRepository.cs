using Domain.Models;

namespace Domain.IRepositories;

public interface IClassroomRepository
{
    Task<List<Classroom>> GetClassroomListAsync(int scheduleGroupId);
    Task<Classroom?> GetClassroomByIdAsync(Guid classroomId);
    Task AddAsync(Classroom classroom, int scheduleGroupId);
    Task UpdateAsync(Classroom classroom);
    Task Delete(Classroom classroom);
}