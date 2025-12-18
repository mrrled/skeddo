using Application.DtoModels;
using Domain;

namespace Application.IServices;

public interface ILessonServices
{
    public Task<List<LessonDto>> GetLessonsByScheduleId(Guid scheduleId);
    public Task<Result<CreateLessonResult>> AddLesson(CreateLessonDto lessonDto, Guid scheduleId);
    public Task<Result<EditLessonResult>> EditLesson(LessonDto lessonDto, Guid scheduleId);
    public Task<Result> DeleteLesson(LessonDto lessonDto, Guid scheduleId);
}