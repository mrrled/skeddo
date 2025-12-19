using Application.DtoModels;
using Domain;

namespace Application.IServices;

public interface ILessonDraftServices
{
    public Task<List<LessonDraftDto>> GetLessonDraftsByScheduleId(Guid scheduleId);
    public Task<Result<LessonDraftDto>> GetLessonDraftById(Guid id);
    public Task<Result<EditLessonResult>> EditDraftLesson(LessonDraftDto lessonDraftDto, Guid scheduleId);
    public Task<Result> DeleteLessonDraft(LessonDraftDto lessonDto, Guid scheduleId);
    public Task<Result> ClearDraftsByScheduleId(Guid scheduleId); // Новый метод
}