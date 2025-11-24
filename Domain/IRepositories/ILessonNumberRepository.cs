using Domain.Models;

namespace Domain.IRepositories;

public interface ILessonNumberRepository
{
    Task<List<LessonNumber>> GetLessonNumbersByScheduleIdAsync(int scheduleId);
    Task AddAsync(LessonNumber lessonNumber, int scheduleId);
    Task UpdateAsync(LessonNumber oldLessonNumber, LessonNumber newLessonNumber, int scheduleId);   //можем поменять только время, но не номер
    Task Delete(LessonNumber lessonNumber, int scheduleId);
}