using Domain.Models;

namespace Domain.IRepositories;

public interface ILessonRepository
{
    Task<List<Lesson>> GetLessonsByScheduleIdAsync(Guid scheduleId);
    Task<Lesson?> GetLessonByIdAsync(Guid id);
    Task<List<Lesson>> GetLessonsByIdsAsync(List<Guid> lessonIds);
    Task AddAsync(Lesson lesson, Guid scheduleId);
    Task UpdateAsync(Lesson lesson);
    Task Delete(Lesson lesson);
    Task UpdateRangeAsync(List<Lesson> lessons);
}