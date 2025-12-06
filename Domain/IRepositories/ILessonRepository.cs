using Domain.Models;

namespace Domain.IRepositories;

public interface ILessonRepository
{
    Task<List<Lesson>> GetLessonsByScheduleIdAsync(int scheduleId);
    Task<Lesson> GetLessonByIdAsync(int id);
    Task<List<Lesson>> GetLessonsByIdsAsync(List<int> lessonIds);
    Task AddAsync(Lesson lesson, int scheduleId);
    Task UpdateAsync(Lesson lesson);
    Task Delete(Lesson lesson);
    Task UpdateRangeAsync(List<Lesson> lessons);
}