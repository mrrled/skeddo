using Application.DtoModels;

namespace Application.IServices;

public interface ILessonDraftServices
{
    public Task<List<LessonDraftDto>> GetLessonDraftsByScheduleId(int scheduleId);
    public Task<LessonDraftDto> GetLessonDraftById(int id);
    public Task<EditLessonResult> EditDraftLesson(LessonDraftDto lessonDraftDto, int scheduleId);
    public Task DeleteLessonDraft(LessonDraftDto lessonDto, int scheduleId);
}