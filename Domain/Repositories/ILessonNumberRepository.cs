using Domain.Models;

namespace Domain.Repositories;

public interface ILessonNumberRepository
{
    Task<List<LessonNumber>> GetLessonNumbersByScheduleIdAsync(int scheduleId);
    Task AddAsync(LessonNumber lessonNumber, int scheduleId);
    Task UpdateAsync(LessonNumber oldLessonNumber, LessonNumber newLessonNumber, int scheduleId);   //можем поменять только время, но не номер
    Task Delete(LessonNumber lessonNumber, int scheduleId);
}