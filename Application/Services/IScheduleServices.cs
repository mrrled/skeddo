using Application.DtoModels;
using Domain.Models;

namespace Application.Services;

public interface IScheduleServices
{
    public Task<List<ScheduleDto>> FetchSchedulesFromBackendAsync();
    public Task AddSchedule(ScheduleDto scheduleDto);
    public Task EditSchedule(ScheduleDto oldScheduleDto, ScheduleDto newScheduleDto);
    public Task DeleteSchedule(ScheduleDto scheduleDto);
}