using Application.DtoModels;
using Application.DtoExtensions;
using Application.IServices;
using Domain.Models;
using Domain.IRepositories;

namespace Application.Services;

public class ScheduleServices(IScheduleRepository scheduleRepository, IUnitOfWork unitOfWork) : IScheduleServices
{
    public async Task<List<ScheduleDto>> FetchSchedulesFromBackendAsync()
    {
        var scheduleList = await scheduleRepository.GetScheduleListAsync(1);
        return scheduleList.ToSchedulesDto();
    }

    public async Task<ScheduleDto> AddSchedule(CreateScheduleDto? scheduleDto)
    {
        if (scheduleDto is null)
            throw new ArgumentNullException(nameof(scheduleDto));
        var id = Guid.NewGuid();
        var schedule = Schedule.CreateSchedule(id, scheduleDto.Name);
        await scheduleRepository.AddAsync(schedule, 1);
        await unitOfWork.SaveChangesAsync();
        return schedule.ToScheduleDto();
    }

    public async Task EditSchedule(ScheduleDto scheduleDto)
    {
        var schedule = await scheduleRepository.GetScheduleByIdAsync(scheduleDto.Id);
        if (schedule is null)
            throw new ArgumentException($"Schedule with id {scheduleDto.Id} not found");
        if (schedule.Name != scheduleDto.Name)
            schedule.SetName(scheduleDto.Name);
        await scheduleRepository.UpdateAsync(schedule);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteSchedule(ScheduleDto scheduleDto)
    {
        var schedule = await scheduleRepository.GetScheduleByIdAsync(scheduleDto.Id);
        if (schedule is null)
            throw new ArgumentException($"Schedule with id {scheduleDto.Id} not found");
        await scheduleRepository.Delete(schedule);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<ScheduleDto> GetScheduleByIdAsync(Guid id)
    {
        var schedule = await scheduleRepository.GetScheduleByIdAsync(id);
        return schedule.ToScheduleDto();
    }
}