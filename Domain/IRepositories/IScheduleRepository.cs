using Domain.Models;

namespace Domain.IRepositories;

public interface IScheduleRepository
{
    Task<List<Schedule>> GetScheduleListAsync();
    Task<Schedule> GetScheduleByIdAsync(int scheduleId);
    Task AddAsync(Schedule schedule);
    Task UpdateAsync(Schedule oldSchedule, Schedule newSchedule);
    Task Delete(Schedule schedule);
}