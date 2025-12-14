using Application.DtoModels;

namespace Application.IServices;

public interface ILessonServices
{
    public Task<List<LessonDto>> GetLessonsByScheduleId(Guid scheduleId);
    public Task<CreateLessonResult> AddLesson(CreateLessonDto lessonDto, Guid scheduleId);
    public Task<EditLessonResult> EditLesson(LessonDto lessonDto, Guid scheduleId);
    public Task DeleteLesson(LessonDto lessonDto, Guid scheduleId);
}