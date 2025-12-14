using Application.DtoModels;
using Domain.Models;

namespace Application.IServices;

public interface IScheduleServices
{
    public Task<List<ScheduleDto>> FetchSchedulesFromBackendAsync();
    public Task<ScheduleDto> AddSchedule(CreateScheduleDto scheduleDto);
    public Task EditSchedule(ScheduleDto scheduleDto);
    public Task DeleteSchedule(ScheduleDto scheduleDto);
    public Task<ScheduleDto> GetScheduleByIdAsync(Guid id);
}