using Application.DtoModels;
using Application.DtoExtensions;
using Application.IServices;
using Domain;
using Domain.Models;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class LessonNumberServices(
    ILessonNumberRepository lessonNumberRepository,
    IScheduleRepository scheduleRepository,
    IUnitOfWork unitOfWork,
    ILogger logger) : BaseService(unitOfWork, logger), ILessonNumberServices
{
    public async Task<List<LessonNumberDto>> GetLessonNumbersByScheduleId(Guid scheduleId)
    {
        var lessonNumbers = await lessonNumberRepository.GetLessonNumbersByScheduleIdAsync(scheduleId);
        return lessonNumbers.ToLessonNumbersDto();
    }

    public async Task<Result> AddLessonNumber(LessonNumberDto lessonNumberDto, Guid scheduleId)
    {
        var schedule = await scheduleRepository.GetScheduleByIdAsync(scheduleId);
        if (schedule is null)
            return Result.Failure("Расписание не найдено");
        var lessonNumberCreateResult = LessonNumber.CreateLessonNumber(lessonNumberDto.Number, lessonNumberDto.Time);
        if (lessonNumberCreateResult.IsFailure)
            return Result.Failure(lessonNumberCreateResult.Error);
        await lessonNumberRepository.AddAsync(lessonNumberCreateResult.Value, scheduleId);
        return await TrySaveChangesAsync("Не удалось сохранить номер урока. Попробуйте позже.");
    }

    public async Task<Result> EditLessonNumber(LessonNumberDto oldLessonNumberDto, LessonNumberDto newLessonNumberDto,
        Guid scheduleId)
    {
        var oldLessonNumberCreateResult = LessonNumber.CreateLessonNumber(oldLessonNumberDto.Number, oldLessonNumberDto.Time);
        if (oldLessonNumberCreateResult.IsFailure)
            return Result.Failure(oldLessonNumberCreateResult.Error);
        var newLessonNumberCreateResult = LessonNumber.CreateLessonNumber(newLessonNumberDto.Number, newLessonNumberDto.Time);
        if (newLessonNumberCreateResult.IsFailure)
            return Result.Failure(newLessonNumberCreateResult.Error);
        await lessonNumberRepository.UpdateAsync(oldLessonNumberCreateResult.Value, newLessonNumberCreateResult.Value, scheduleId);
        return await TrySaveChangesAsync("Не удалось изменить номер урока. Попробуйте позже.");
    }

    public async Task<Result> DeleteLessonNumber(LessonNumberDto lessonNumberDto, Guid scheduleId)
    {
        var schedule = await scheduleRepository.GetScheduleByIdAsync(scheduleId);
        if (schedule is null)
            return Result.Failure("Расписание не найдено");
        var lessonNumberCreateResult = LessonNumber.CreateLessonNumber(lessonNumberDto.Number, lessonNumberDto.Time);
        await lessonNumberRepository.Delete(lessonNumberCreateResult.Value, scheduleId);
        return await TrySaveChangesAsync("Не удалось удалить номер урока. Попробуйте позже.");
    }
}