using Application.DtoModels;
using Domain;
using Domain.Models;

namespace Application.IServices;

public interface IScheduleServices
{
    public Task<List<ScheduleDto>> FetchSchedulesFromBackendAsync();
    public Task<Result<ScheduleDto>> AddSchedule(CreateScheduleDto scheduleDto);
    public Task<Result> EditSchedule(ScheduleDto scheduleDto);
    public Task<Result> DeleteSchedule(ScheduleDto scheduleDto);
    public Task<Result<ScheduleDto>> GetScheduleByIdAsync(Guid id);
}