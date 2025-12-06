using Domain.Models;

namespace Domain.IRepositories;

public interface ILessonDraftRepository
{
    Task AddAsync(LessonDraft lessonDraft, int scheduleId);
    Task<List<LessonDraft>> GetLessonDraftsByScheduleId(int scheduleId);
    Task<LessonDraft> GetLessonDraftById(int id);
    Task Delete(LessonDraft lessonDraft);
    Task Update(LessonDraft lessonDraft);
}