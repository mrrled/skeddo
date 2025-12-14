using Domain.Models;

namespace Domain.IRepositories;

public interface ILessonDraftRepository
{
    Task AddAsync(LessonDraft lessonDraft, Guid scheduleId);
    Task<List<LessonDraft>> GetLessonDraftsByScheduleId(Guid scheduleId);
    Task<LessonDraft> GetLessonDraftById(Guid id);
    Task Delete(LessonDraft lessonDraft);
    Task Update(LessonDraft lessonDraft);
}