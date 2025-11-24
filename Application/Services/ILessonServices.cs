using Application.DtoModels;
using Domain.Models;

namespace Application.Services;

public interface ILessonServices
{
    public Task<List<LessonDto>> GetLessonsByScheduleId(int scheduleId);
    public Task AddLesson(LessonDto lessonDto, int scheduleId);
    public Task EditLesson(LessonDto lessonDto, int scheduleId);
    public Task DeleteLesson(LessonDto lessonDto, int scheduleId);
}