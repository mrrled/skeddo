using Application.DtoModels;
using Domain.Models;

namespace Application.IServices;

public interface IScheduleServices
{
    public Task<List<ScheduleDto>> FetchSchedulesFromBackendAsync();
    public Task<ScheduleDto> FetchScheduleByIdAsync(int id);
    public Task AddSchedule(ScheduleDto scheduleDto);
    public Task EditSchedule(ScheduleDto scheduleDto);
    public Task DeleteSchedule(ScheduleDto scheduleDto);
    public Task<ScheduleDto> GetScheduleByIdAsync(int id);
}