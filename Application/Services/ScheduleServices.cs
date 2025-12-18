using Application.DtoModels;
using Application.DtoExtensions;
using Application.IServices;
using Domain;
using Domain.Models;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class ScheduleServices(IScheduleRepository scheduleRepository, IUnitOfWork unitOfWork, ILogger logger) : BaseService(unitOfWork, logger), IScheduleServices
{
    public async Task<List<ScheduleDto>> FetchSchedulesFromBackendAsync()
    {
        var scheduleList = await scheduleRepository.GetScheduleListAsync(1);
        return scheduleList.ToSchedulesDto();
    }

    public async Task<Result<ScheduleDto>> AddSchedule(CreateScheduleDto scheduleDto)
    {
        var id = Guid.NewGuid();
        var scheduleCreateResult = Schedule.CreateSchedule(id, scheduleDto.Name);
        if (scheduleCreateResult.IsFailure)
            return Result<ScheduleDto>.Failure(scheduleCreateResult.Error);
        await scheduleRepository.AddAsync(scheduleCreateResult.Value, 1);
        return await TrySaveChangesAsync(scheduleCreateResult.Value.ToScheduleDto(),
            "Не удалось сохранить расписание.  Попробуйте позже.");
    }

    public async Task<Result> EditSchedule(ScheduleDto scheduleDto)
    {
        var schedule = await scheduleRepository.GetScheduleByIdAsync(scheduleDto.Id);
        if (schedule is null)
            return Result.Failure("Расписание не найдено. Попробуйте позже.");
        if (schedule.Name != scheduleDto.Name)
        {
            var renameResult = schedule.SetName(scheduleDto.Name);
            if (renameResult.IsFailure)
                return Result.Failure(renameResult.Error);
        }
        await scheduleRepository.UpdateAsync(schedule);
        return await TrySaveChangesAsync("Не удалось изменить расписание. Попробуйте позже.");
    }

    public async Task<Result> DeleteSchedule(ScheduleDto scheduleDto)
    {
        var schedule = await scheduleRepository.GetScheduleByIdAsync(scheduleDto.Id);
        if (schedule is null)
            return Result.Failure("Расписание не найдено. Попробуйте позже.");
        await scheduleRepository.Delete(schedule);
        return await TrySaveChangesAsync("Не удалось удалить расписание. Попробуйте позже.");
    }

    public async Task<Result<ScheduleDto>> GetScheduleByIdAsync(Guid id)
    {
        var schedule = await scheduleRepository.GetScheduleByIdAsync(id);
        if (schedule is null)
            return Result<ScheduleDto>.Failure("Расписание не найдено");
        return Result<ScheduleDto>.Success(schedule.ToScheduleDto());
    }
}