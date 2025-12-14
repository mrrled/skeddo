using Application.DtoModels;
using Domain.Models;

namespace Application.IServices;

public interface ILessonNumberServices
{
    public Task<List<LessonNumberDto>> GetLessonNumbersByScheduleId(Guid scheduleId);
    public Task AddLessonNumber(LessonNumberDto lessonNumberDto, Guid scheduleId);
    public Task EditLessonNumber(LessonNumberDto oldLessonNumberDto, LessonNumberDto newLessonNumberDto, Guid scheduleId);
    public Task DeleteLessonNumber(LessonNumberDto lessonNumberDto, Guid scheduleId);
}