using Domain.Models;

namespace Domain.IRepositories;

public interface IScheduleRepository
{
    Task<List<Schedule>> GetScheduleListAsync(int scheduleGroupId);
    Task<Schedule> GetScheduleByIdAsync(int scheduleId);
    Task AddAsync(Schedule schedule, int scheduleGroupId);
    Task UpdateAsync(Schedule schedule);
    Task Delete(Schedule schedule);
}