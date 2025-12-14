using Domain.Models;

namespace Domain.IRepositories;

public interface ILessonNumberRepository
{
    Task<List<LessonNumber>> GetLessonNumbersByScheduleIdAsync(Guid scheduleId);
    Task AddAsync(LessonNumber lessonNumber, Guid scheduleId);
    Task UpdateAsync(LessonNumber oldLessonNumber, LessonNumber newLessonNumber, Guid scheduleId);   //можем поменять только время, но не номер
    Task Delete(LessonNumber lessonNumber, Guid scheduleId);
}