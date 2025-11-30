using Domain.Models;

namespace Domain.IRepositories;

public interface ILessonRepository
{
    Task<List<Lesson>> GetLessonsByScheduleIdAsync(int scheduleId);
    Task<Lesson> GetLessonByIdAsync(int id, int scheduleId);
    Task AddAsync(Lesson lesson, int scheduleId);
    Task UpdateAsync(Lesson lesson, int scheduleId);
    Task Delete(Lesson lesson, int scheduleId);
    Task UpdateRangeAsync(List<Lesson> lessons, int scheduleId);
}