using Application.DtoModels;

namespace Application.IServices;

public interface ILessonDraftServices
{
    public Task<List<LessonDraftDto>> GetLessonDraftsByScheduleId(Guid scheduleId);
    public Task<LessonDraftDto> GetLessonDraftById(Guid id);
    public Task<EditLessonResult> EditDraftLesson(LessonDraftDto lessonDraftDto, Guid scheduleId);
    public Task DeleteLessonDraft(LessonDraftDto lessonDto, Guid scheduleId);
}