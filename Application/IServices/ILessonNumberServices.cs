using Application.DtoModels;
using Domain;
using Domain.Models;

namespace Application.IServices;

public interface ILessonNumberServices
{
    public Task<List<LessonNumberDto>> GetLessonNumbersByScheduleId(Guid scheduleId);
    public Task<Result> AddLessonNumber(LessonNumberDto lessonNumberDto, Guid scheduleId);
    public Task<Result> EditLessonNumber(LessonNumberDto oldLessonNumberDto, LessonNumberDto newLessonNumberDto, Guid scheduleId);
    public Task<Result> DeleteLessonNumber(LessonNumberDto lessonNumberDto, Guid scheduleId);
}