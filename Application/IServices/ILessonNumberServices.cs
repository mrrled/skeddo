using Application.DtoModels;
using Domain.Models;

namespace Application.IServices;

public interface ILessonNumberServices
{
    public Task<List<LessonNumberDto>> GetLessonNumbersByScheduleId(int scheduleId);
    public Task AddLessonNumber(LessonNumberDto lessonNumberDto, int scheduleId);
    public Task EditLessonNumber(LessonNumberDto oldLessonNumberDto, LessonNumberDto newLessonNumberDto, int scheduleId);
    public Task DeleteLessonNumber(LessonNumberDto lessonNumberDto, int scheduleId);
}